using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Jobsway2goMvc.Extensions;

public static class LinkExtensions
{
    public static IHtmlContent ActiveActionLink(this IHtmlHelper html, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
    {
        return ActiveActionLink(html, linkText, actionName, controllerName, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static IHtmlContent ActiveActionLink(this IHtmlHelper html, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
    {
        var routeData = html.ViewContext.RouteData;
        var routeAction = (string)routeData.Values["action"];
        var routeController = (string)routeData.Values["controller"];

        var active = controllerName.Equals(routeController);

        using (var writer = new StringWriter())
        {
            writer.WriteLine($"<li class='nav-item {(active ? "active" : "")}'>");
            html.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes).WriteTo(writer, HtmlEncoder.Default);
            writer.WriteLine("</li>");
            return new HtmlString(writer.ToString());
        }
    }
}