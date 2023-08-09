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

namespace ActionableEmailsTestApi.Controllers
{
    public class ValuesController : ApiController
    {
        static string test = "";
        static string requestTest = "";

        static string cardSender = "cardSender: {0}";
        static string actionSender = "actionSender: {0}";

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { test, requestTest, cardSender, actionSender };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public async Task<HttpResponseMessage> Post([FromBody] string value)
        {
            HttpRequestMessage request = this.ActionContext.Request;

            test = value;

            if (request.Headers.Authorization != null)
            {
                requestTest = request.Headers.Authorization.ToString();

                // Validate that we have a bearer token.
                if (request.Headers.Authorization == null ||
                    !string.Equals(request.Headers.Authorization.Scheme, "bearer", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrEmpty(request.Headers.Authorization.Parameter))
                {
                    return request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError());
                }

                // Get the token from the Authorization header 
                string bearerToken = request.Headers.Authorization.Parameter;

                ActionableMessageTokenValidator validator = new ActionableMessageTokenValidator();

                // This will validate that the token has been issued by Microsoft for the
                // specified target URL i.e. the target matches the intended audience (“aud” claim in token)
                // 
                // In your code, replace https://api.contoso.com with your service’s base URL.
                // For example, if the service target URL is https://api.xyz.com/finance/expense?id=1234,
                // then replace https://api.contoso.com with https://api.xyz.com
                ActionableMessageTokenValidationResult result = await validator.ValidateTokenAsync(bearerToken, "https://astionablemessagestestframework.azurewebsites.net");

                if (!result.ValidationSucceeded)
                {
                    if (result.Exception != null)
                    {
                        Trace.TraceError(result.Exception.ToString());
                    }

                    return request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError());
                }



                // We have a valid token. We will now verify that the sender and action performer are who
                // we expect. The sender is the identity of the entity that initially sent the Actionable 
                // Message, and the action performer is the identity of the user who actually 
                // took the action (“sub” claim in token). 
                // 
                // You should replace the code below with your own validation logic 
                // In this example, we verify that the email is sent by expense@contoso.com (expected sender)
                // and the email of the person who performed the action is john@contoso.com (expected recipient)
                //
                // You should also return the CARD-ACTION-STATUS header in the response.
                // The value of the header will be displayed to the user.

                string.Format(cardSender, result.Sender);
                string.Format(actionSender, result.ActionPerformer);
                

                // Further business logic code here to process the expense report.
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("CARD-ACTION-STATUS", "The expense was approved.");

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
