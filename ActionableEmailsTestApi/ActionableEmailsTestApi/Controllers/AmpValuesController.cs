﻿using System;
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
using ActionableEmailsTestApi.Models;

namespace ActionableEmailsTestApi.Controllers
{
    public class AmpValuesController : ApiController
    {
        static string test = "";
        static string requestTest = "";
        static string cardSender = "";
        static string actionSender = "";
        static List<string> headers = new List<string>();

        static string cardSenderString = "cardSender: {0}";
        static string actionSenderString = "actionSender: {0}";

        // GET api/values
        public List<string> Get()
        {
            return headers;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        // POST api/values
        public HttpResponseMessage Post(string __amp_source_origin , [FromBody] AmpModel model)
        {
            HttpRequestMessage request = this.ActionContext.Request;
            foreach (var header in request.Headers)
            {
                var headerValuesString = string.Empty;
                foreach(var value in header.Value)
                {
                    headerValuesString += value;
                }
                var headerString = string.Format( "{0} : {1}", header.Key, headerValuesString);
                headers.Add(headerString);                
            }

            var modelString = string.Format("value : {0}", model.Value);
            headers.Add(modelString);
            test = model.Value;

            

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
