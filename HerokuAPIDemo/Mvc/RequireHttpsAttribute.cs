using System;
using System.Web;
using System.Web.Mvc;
using RequireHttpsAttributeBase = System.Web.Mvc.RequireHttpsAttribute;

namespace HerokuAPIDemo.Mvc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,
		AllowMultiple = false)]
	public class RequireHttpsAttribute : RequireHttpsAttributeBase
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.Request.IsSecureConnection)
			{
				return;
			}

			if (string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"],
				"https",
				StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}

			if (filterContext.HttpContext.Request.IsLocal)
			{
				return;
			}

			HandleNonHttpsRequest(filterContext);
		}

		protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
		{
			if (!string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
			{
				throw new HttpException(403, "forbidden");
			}

			base.HandleNonHttpsRequest(filterContext);
		}
	}
}
