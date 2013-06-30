using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using RequireHttpsAttribute = HerokuAPIDemo.Mvc.RequireHttpsAttribute;

namespace HerokuAPIDemo
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			GlobalFilters.Filters.Add(new RequireHttpsAttribute());
		}
	}
}
