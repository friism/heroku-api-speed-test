using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using HerokuAPIDemo.Mvc;
using System.Diagnostics;

namespace HerokuAPIDemo.Controllers
{
	public class AppController : Controller
	{
		private const string _tokenKey = "Access_Token";

		[ActionTimer]
		public ActionResult Index()
		{
			var token = HttpContext.Session[_tokenKey];
			if (token == null)
			{
				return RedirectToAction("Index", "Home");
			}

			var authorization = string.Format("{0}", Convert.ToBase64String(
				Encoding.ASCII.GetBytes(string.Format(":{0}", token))));

			var results = new ConcurrentDictionary<string, bool>();
			using (var requestHandler = new HttpClientHandler {
				UseCookies = false,
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
				})
			using (var client = new HttpClient(requestHandler))
			{
				client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.heroku+json; version=3");
				client.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Basic", authorization);
				client.BaseAddress = new Uri("https://api.heroku.com");

				var watch = Stopwatch.StartNew();
				var apps = JsonConvert.DeserializeObject<IEnumerable<App>>(
					client.GetAsync("apps").Result.Content.ReadAsStringAsync().Result);
				watch.Stop();
				ViewBag.TimeToFetchApps = watch.ElapsedMilliseconds;

				watch.Restart();
				var tasks = apps.Select(x =>
					client.GetAsync(string.Format("apps/{0}/dynos", x.Name))
						.ContinueWith(task =>
							{
								var dynos = JsonConvert.DeserializeObject<IEnumerable<Dyno>>(
									task.Result.Content.ReadAsStringAsync().Result);
								results.AddOrUpdate(
									x.Name, dynos.Any(y => y.State == "idle"), (s, b) => { throw new Exception(); });
							}));
				Task.WaitAll(tasks.ToArray());
				watch.Stop();
				ViewBag.TimeToFetchAppStates = watch.ElapsedMilliseconds;
				return View(results);
			};
		}
	}
}
