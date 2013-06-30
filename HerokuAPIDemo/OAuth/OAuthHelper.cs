using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Authentication;

namespace HerokuAPIDemo.OAuth
{
	// REMARK: This code is taken from the AppHarbor SDK, https://github.com/appharbor/AppHarbor.NET/blob/master/AppHarbor.Sdk/AppHarborClient.Auth.cs
	public class OAuthHelper
	{
		private const string BaseUrl = "https://id.heroku.com/";

		public static AuthInfo GetAuthInfo(string clientId, string clientSecret, string code)
		{
			// make POST request to obtain the token
			var client = new RestClient(BaseUrl);
			client.AddHandler("application/json", new DynamicJsonDeserializer());
			var request = new RestRequest(Method.POST);
			request.Resource = "oauth/token";
			request.AddParameter("client_id", clientId);
			request.AddParameter("client_secret", clientSecret);
			request.AddParameter("code", code);

			var response = client.Execute<dynamic>(request);
			if (response.StatusCode != HttpStatusCode.OK)
			{
				return null;
			}

			return new AuthInfo(response.Data.access_token.ToString(), response.Data.token_type.ToString());
		}
	}
}
