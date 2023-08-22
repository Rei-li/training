using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.O365.ActionableMessages.Utilities;

namespace ActionableEmailsTestApi.Controllers
{
    [RoutePrefix("api/AmpValues")]
    public class AmpValuesController : ApiController
    {
        static string test = "";
        static string requestTest = "";
        static string cardSender = "";
        static string actionSender = "";
        static List<string> claims = new List<string>();

        static string cardSenderString = "cardSender: {0}";
        static string actionSenderString = "actionSender: {0}";

        [HttpGet]
        [Route("get")]
        public string Get()
        {
            return test;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Post(string value)
        {
            HttpRequestMessage request = this.ActionContext.Request;

            test = value.ToString();

            

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "https://astionablemessagestestframework.azurewebsites.net");
            response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", "https://amp.gmail.dev");
            response.Headers.Add("Access-Control-Expose-Headers", "AMP-Access-Control-Allow-Source-Origin");

            return response;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
