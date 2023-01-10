using FluentValidation;
using FluentValidation.AspNetCore;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Extensions;
using Jobsway2goMvc.Hubs;
using Jobsway2goMvc.Interfaces;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Services;
using Jobsway2goMvc.SubscribeTableDependencies;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()))
            .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false);

var connectionString = builder.Configuration.GetConnectionString("JobPortalConString");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Scoped
);


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(UserProfileMapper).Assembly);
builder.Services.AddRazorPages();

//notification SignalR
builder.Services.AddSignalR();

builder.Services.AddScoped<NotificationHub>();
builder.Services.AddScoped<ISubscribeTableDependency,SubscribeNotificationTableDependency>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");
app.UseSqlTableDependency<SubscribeNotificationTableDependency>(connectionString);

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
