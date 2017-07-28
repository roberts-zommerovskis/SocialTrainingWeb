using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using DataModel.DataContext;
using DataModel.DataModel;

namespace Api.Security
{
    public class OAuth2AuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        // Use https://developers.google.com/+/web/api/rest/latest/people/get
        // to obtain an Access Token. It is sent in Authentication HTTP header. You need to send the same HTTP header to our system.
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var authorization = context.Request.Headers.Authorization;

                if (authorization == null)
                {
                    if (string.Compare(ConfigurationManager.AppSettings["AnonymousMode"], "true", true) == 0)
                    {
                        GenericIdentity identity = new GenericIdentity("anonymous", "None");

                        string[] roles = new string[0];

                        if (string.Compare(ConfigurationManager.AppSettings["AnonymousAdminMode"], "true", true) == 0)
                            roles = new string[] { Role.Admin.ToString() };

                        context.Principal = new GenericPrincipal(identity, roles);
                    }
                    return;
                }

                if (!string.Equals(authorization.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                    return;

                OAuth2Validator validator = new OAuth2Validator();

                string email = validator.Validate(authorization.Parameter);

                if (email == null)
                    return;

                using (SocialTrainingContext db = new SocialTrainingContext())
                {
                    User user = db.Users.FirstOrDefault(x => x.Email == email);

                    if (user == null)
                        return;

                    GenericIdentity identity = new GenericIdentity(email, "OAuth2");
                    string[] roles = db.UserRoles.Where(x => x.UserId == user.UserId).Select(x => x.Role.ToString()).ToArray();

                    context.Principal = new GenericPrincipal(identity, roles);
                }
            });
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}