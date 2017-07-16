using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace paxa.Utilities
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        private String CLIENT_ID = "208762726005-snfujhcqdcu40gkla949jlakd1pphmpk.apps.googleusercontent.com";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            string auth = req.Headers["Authorization"];
            if (!String.IsNullOrEmpty(auth))
            {
                string[] split = Regex.Split(auth.Trim(), "\\s+");
                if (split == null || split.Length != 2 || !string.Equals(split[0], "Bearer", StringComparison.CurrentCultureIgnoreCase))
                {
                    filterContext.Result = new Http401Result();
                    //Autentiseringsproblem. Felaktigt angivet token.
                }

                String token = split[1];
                // Check if the token is present
                if (string.IsNullOrEmpty(token))
                {
                    filterContext.Result = new Http401Result();
                    //Autentiseringsproblem. Token saknas.
                }

                // Validate the token
                validateToken(token, filterContext);
            }
            base.OnActionExecuting(filterContext);
        }

        private void validateToken(String token, ActionExecutingContext filterContext)
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
                var controller = filterContext.Controller as Controller;

                if (resp != null && resp.IsSuccessStatusCode && controller != null)
                {
                    var jsonString = resp.Content.ReadAsStringAsync();
                    jsonString.Wait(1000);
                    TokenInfo tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(jsonString.Result);

                    controller.ViewBag.ProfileId = tokenInfo.sub;
                    controller.ViewBag.ProfileName = tokenInfo.name;
                    controller.ViewBag.ProfileEmail = tokenInfo.email;
                }
                else
                {
                    // Lyckades inte kontakt Google för autentisering!
                    filterContext.Result = new Http401Result();
                }
            }
            catch (Exception e)
            {
                //("Autentiseringsproblem: " + e.getMessage()
                filterContext.Result = new Http401Result();
            }
        }

        internal class Http401Result : ActionResult
        {
            public override void ExecuteResult(ActionContext context)
            {
                // Set the response code to 401.
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
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
