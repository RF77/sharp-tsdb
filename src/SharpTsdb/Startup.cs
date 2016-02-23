using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Filters;
using Owin;

namespace SharpTsdb
{
    //public class AddCustomHeaderFilter : ActionFilterAttribute
    //{
    //    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    //    {
    //        actionExecutedContext.Response.Headers.Add("X-Influxdb-Version", "0.9.2");
    //    }
    //}
    class Startup
    {
        //  Hack from http://stackoverflow.com/a/17227764/19020 to load controllers in 
        //  another assembly.  Another way to do this is to create a custom assembly resolver
        //Type valuesControllerType = typeof(OWINTest.API.ValuesController);

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //  Enable attribute based routing
            //  http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //config.Filters.Add(new AddCustomHeaderFilter());
            

            appBuilder.UseWebApi(config);
        }

    }
}