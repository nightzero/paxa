using Google.Apis.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace paxa.Utilities
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        private String CLIENT_ID = "208762726005-snfujhcqdcu40gkla949jlakd1pphmpk.apps.googleusercontent.com";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var req = filterContext.Request;
            IEnumerable<string> authValues = req.Headers.GetValues("Authorization");
            string auth = authValues.First();

            if (!String.IsNullOrEmpty(auth))
            {
                string[] split = Regex.Split(auth.Trim(), "\\s+");
                if (split == null || split.Length != 2 || !string.Equals(split[0], "Bearer", StringComparison.CurrentCultureIgnoreCase))
                {
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Autentiseringsproblem. Felaktigt angivet token.");
                }

                String token = split[1];
                // Check if the token is present
                if (string.IsNullOrEmpty(token))
                {
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Autentiseringsproblem. Token saknas.");
                }

                // Validate the token
                validateTokenAsync(token, filterContext);
            }
            base.OnActionExecuting(filterContext);
        }

        /**
         * Gamla sättet att anropa authorization.
         */
        [Obsolete("validateToken is deprecated, please use validateTokenAsync instead.")]
        private void validateToken(String token, HttpActionContext filterContext)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var respTask = client.GetAsync("tokeninfo?id_token=" + token);
                respTask.Wait(3000);
                HttpResponseMessage resp = respTask.Result;

                if (resp != null && resp.IsSuccessStatusCode)
                {
                    var jsonString = resp.Content.ReadAsStringAsync();
                    jsonString.Wait(1000);
                    TokenInfo tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(jsonString.Result);

                    filterContext.Request.Properties.Add("ProfileId", tokenInfo.sub);
                    filterContext.Request.Properties.Add("ProfileName", tokenInfo.name);
                    filterContext.Request.Properties.Add("ProfileEmail", tokenInfo.email);
                }
                else
                {
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Lyckades inte kontakta Google för autentisering. Har du loggat in?");
                }
            }
            catch (Exception e)
            {
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Autentiseringsproblem: " + e.Message);
            }
        }

        private void validateTokenAsync(String token, HttpActionContext filterContext)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings() { Audience = new List<string>() { CLIENT_ID } };

            try
            {
                //var validPayload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                //Verkar vara problem med asynkrona anrop på serversidan. Hack för att undvika det.
                var validPayload = Task.Run(() => GoogleJsonWebSignature.ValidateAsync(token, settings)).GetAwaiter().GetResult();
                filterContext.Request.Properties.Add("ProfileId", validPayload.Subject);
                filterContext.Request.Properties.Add("ProfileName", validPayload.Name);
                filterContext.Request.Properties.Add("ProfileEmail", validPayload.Email);
            }
            catch (InvalidJwtException)
            {
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Lyckades inte kontakta Google för autentisering. Har du loggat in? ");
            }
            catch (Exception e)
            {
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Autentiseringsproblem: " + e.Message);
            }
        }

        internal class TokenInfo
        {
            public string sub { get; set; }
            public string aud { get; set; }
            public string email { get; set; }
            public string name { get; set; }
        }
    }
}
