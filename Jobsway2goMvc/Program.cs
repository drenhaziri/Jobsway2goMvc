using FluentValidation;
using FluentValidation.AspNetCore;
using Jobsway2goMvc.Areas.Identity.Pages.Account;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Hubs;
//using Jobsway2goMvc.MiddlewareExtensions;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Services;
using Jobsway2goMvc.SubscribeTableDependencies;
using Jobsway2goMvc.Validators.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("JobPortalConString");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString),
            ServiceLifetime.Scoped
        );
        builder.Services.AddScoped<IValidator<RegisterModel.InputModel>, RegistrationValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<RegistrationValidator>();


        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

        builder.Services.AddAutoMapper(typeof(UserProfileMapper).Assembly);
        builder.Services.AddRazorPages();

        //notification SignalR
        builder.Services.AddSignalR();

        builder.Services.AddScoped<NotificationHub>();
        builder.Services.AddScoped<ISubscribeTableDependency, SubscribeNotificationTableDependency>();

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
        //app.UseSqlTableDependency<SubscribeNotificationTableDependency>(connectionString);

        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}