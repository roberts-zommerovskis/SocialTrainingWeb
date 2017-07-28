using System;
using System.Configuration;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;

namespace Api.Security
{
    // Based on Google-provided solution by Gus Class.
    public class OAuth2Validator
    {
        public string Validate(string accessToken)
        {
            string ClientId = ConfigurationManager.AppSettings["GoogleClientId"];

            // Use Tokeninfo to validate the user and the client.
            Oauth2Service.TokeninfoRequest tokenInfoRequest = new Oauth2Service().Tokeninfo();
            tokenInfoRequest.AccessToken = accessToken;

            // Use Google as a trusted provider to validate the token.
            // Invalid values, including expired tokens, return 400
            Tokeninfo tokenInfo = null;
            try
            {
                tokenInfo = tokenInfoRequest.Execute();
                if (tokenInfo.IssuedTo != ClientId)
                {
                    return null;
                }
                return tokenInfo.Email;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}