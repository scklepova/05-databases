using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Coordinator.IoC;
using Core;
using Owin;

namespace Coordinator
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration {DependencyResolver = new DependencyResolver(IoCFactory.GetContainer())};
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional});
            config.Services.Replace(typeof (IExceptionLogger), new ConsoleExceptionLogger());
            appBuilder.Use<LogMiddleware>();
            appBuilder.UseWebApi(config);
        }
    }
}