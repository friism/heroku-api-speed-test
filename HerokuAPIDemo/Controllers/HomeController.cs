using HerokuAPIDemo.OAuth;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace HerokuAPIDemo.Controllers
{
	public class HomeController : Controller
	{
		private const string _tokenKey = "Access_Token";
		private const string _clientIdKey = "ClientId";

		public ActionResult Index()
		{
			var token = HttpContext.Session[_tokenKey];
			if (token != null)
			{
				return RedirectToAction("Index", "App");
			}

			ViewBag.AuthLink = string.Format("https://id.heroku.com/oauth/authorize?client_id={0}&response_type=code",
				ConfigurationManager.AppSettings[_clientIdKey]);
			return View();
		}

		public ActionResult Auth(string code)
		{
			AuthInfo authInfo = null;
			authInfo = OAuthHelper.GetAuthInfo(ConfigurationManager.AppSettings[_clientIdKey],
				ConfigurationManager.AppSettings["ClientSecret"], code);

			if (authInfo != null)
			{
				HttpContext.Session[_tokenKey] = authInfo.AccessToken;
			}

			return RedirectToAction("Index");
		}
	}
}
