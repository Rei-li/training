using Microsoft.O365.ActionableMessages.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionableEmailsTestApi
{
    public class CustomActionableMessageTokenValidationResult : ActionableMessageTokenValidationResult
    {
        public List<string> OtherClaims { get; set; }
    }
}