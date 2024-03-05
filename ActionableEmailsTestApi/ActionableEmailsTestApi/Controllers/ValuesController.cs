using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ActionableEmailsTestApi.Models;
using System.Text;
using AdaptiveCards;
using System.Text.RegularExpressions;

namespace ActionableEmailsTestApi.Controllers
{
    public class ValuesController : ApiController
    {
        static string test = "";
        static List <Dictionary<string, string>> testDict = new List<Dictionary<string, string>>();
        static string requestTest = "";
        static string cardSender = "";
        static string actionSender = "";
        static List<string> claims = new List<string>();

        static string cardSenderString = "cardSender: {0}";
        static string actionSenderString = "actionSender: {0}";

        // GET api/values
        public IEnumerable<string> Get()
        {

            //var result = new List<string> { requestTest, cardSender, actionSender };

            //foreach(var claim in claims)
            //{
            //    result.Add(claim);
            //}

            var result = new List<string>();
            var counter = 1;
            foreach (var value in testDict)
            {
                
                foreach (var dict in value)
                {
                    result.Add("Response " + counter  + " "+ dict.Key + ": " + dict.Value);
                }
                counter++;
                
            }

            return result;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public async Task<HttpResponseMessage> Post([FromBody] CommentModel model)
        {
            HttpRequestMessage request = this.ActionContext.Request;           

            var dict =  new Dictionary<string, string>();
            foreach (var t in model.Data)
            {
                dict.Add(t.Key, t.Value);
            }
            testDict.Add(dict);


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

                CustomActionableMessageTokenValidator validator = new CustomActionableMessageTokenValidator();

                // This will validate that the token has been issued by Microsoft for the
                // specified target URL i.e. the target matches the intended audience (“aud” claim in token)
                // 
                // In your code, replace https://api.contoso.com with your service’s base URL.
                // For example, if the service target URL is https://api.xyz.com/finance/expense?id=1234,
                // then replace https://api.contoso.com with https://api.xyz.com
                CustomActionableMessageTokenValidationResult result = await validator.CustomValidateTokenAsync(bearerToken, "https://astionablemessagestestframework.azurewebsites.net");

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

                cardSender = string.Format(cardSenderString, result.Sender);
                actionSender = string.Format(actionSenderString, result.ActionPerformer);
                claims = result.OtherClaims;

                // Further business logic code here to process the expense report.
            }


            var errorMessage = string.Empty;
            int updateEntry;
            if(model.Data.TryGetValue("entryId", out string entryId) && int.TryParse( entryId, out int entryIdValue) && entryIdValue > 0)
            {
                updateEntry = entryIdValue;
            }
            else
            {
                errorMessage = errorMessage + " Wrong entry.";
            }

            int updateField;
            if (model.Data.TryGetValue("fieldId", out string fieldId) && int.TryParse(fieldId, out int fieldIdValue) && fieldIdValue > 0)
            {
                updateField = fieldIdValue;
            }
            else
            {
                errorMessage = errorMessage + "wrong field";
            }

            var valueToSet = string.Empty;
            if (model.Data.TryGetValue("value", out string value))
            {
                valueToSet = value;
            }


            var card = AdaptiveCard.FromJson(model.Card.ToString());


            AdaptiveContainer containerToUpdate = null;

            foreach (var item in card.Card.Body)
            {
                var container = FindContainerToUpdate(item, model.ContainerId);
                if (container != null)
                {
                    containerToUpdate = container;
                    break;
                }
            }

            if (containerToUpdate != null)
            {
                containerToUpdate.Items.Clear();
                containerToUpdate.Items.Add(new AdaptiveTextBlock
                {
                    Text = "Value submitted successfully.",
                    Color = AdaptiveTextColor.Good,
                    Wrap = true,
                    Weight = AdaptiveTextWeight.Bolder
                });
            }


            // var responceJson = "{\r\n    \"type\": \"AdaptiveCard\",\r\n    \"body\": [\r\n        {\r\n            \"type\": \"TextBlock\",\r\n            \"size\": \"Medium\",\r\n            \"weight\": \"Bolder\",\r\n            \"text\": \"Your responce was processed\"\r\n        }\r\n    ],\r\n    \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\",\r\n    \"version\": \"1.0\"\r\n}";

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("CARD-ACTION-STATUS", "Accepted");
            response.Headers.Add("CARD-UPDATE-IN-BODY", "true");
            response.Content = new StringContent(card.ToString(), Encoding.UTF8, "application/json");        

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

        private AdaptiveContainer FindContainerToUpdate(AdaptiveElement element, string elementId)
        {
            if(element is AdaptiveContainer container)
            {
                if (container.Id == elementId)
                {
                    return container;
                }
                else
                {
                    foreach (var item in container)
                    {
                        return FindContainerToUpdate(item, elementId);
                    }
                }
                                              
            }
            return null;
        }
    }
}
