using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.O365.ActionableMessages.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace ActionableEmailsTestApi
{
    public class CustomActionableMessageTokenValidator : ActionableMessageTokenValidator
    {
        public async Task<CustomActionableMessageTokenValidationResult> CustomValidateTokenAsync(string token, string targetServiceBaseUrl)
        {
            var result = new CustomActionableMessageTokenValidationResult();
            var baseResult = await base.ValidateTokenAsync(token, targetServiceBaseUrl);

            result.ActionPerformer = baseResult.ActionPerformer;
            result.Sender = baseResult.Sender;
            result.Exception = baseResult.Exception;
            result.ValidationSucceeded = baseResult.ValidationSucceeded;

            CancellationToken cancel = default(CancellationToken);
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>("https://substrate.office.com/sts/common/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConnectConfiguration = await configurationManager.GetConfigurationAsync(cancel);

            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters();
            tokenValidationParameters.ValidateIssuer = true;
            tokenValidationParameters.ValidIssuers = new string[1] { "https://substrate.office.com/sts/" };
            tokenValidationParameters.ValidateAudience = true;
            tokenValidationParameters.ValidAudiences = new string[1] { targetServiceBaseUrl };
            tokenValidationParameters.ValidateLifetime = true;
            tokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5.0);
            tokenValidationParameters.RequireSignedTokens = true;
            tokenValidationParameters.IssuerSigningKeys = openIdConnectConfiguration.SigningKeys;
            TokenValidationParameters validationParameters = tokenValidationParameters;

            SecurityToken validatedToken = null;
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            ClaimsIdentity claimsIdentity = claimsPrincipal.Identities.OfType<ClaimsIdentity>().FirstOrDefault();

            result.OtherClaims = new List<string>();
            foreach (var claim in claimsIdentity.Claims)
            {
                var subject = claim.Subject;
                var type = claim.Type;
                var value = claim.Value;

                var claimMessage = "Subject: {0}; Type: {1}; Value: {2} ";

                result.OtherClaims.Add(string.Format(claimMessage, subject, type, value));
            }

            return result;
        }
    }
}