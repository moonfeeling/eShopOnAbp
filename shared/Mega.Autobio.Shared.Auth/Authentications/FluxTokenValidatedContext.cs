using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Mega.Autobio.Shared.Auth.Authentications
{
   public class FluxTokenValidatedContext : ResultContext<AuthenticationSchemeOptions>
    {
        public FluxTokenValidatedContext(HttpContext context, AuthenticationScheme scheme,
            AuthenticationSchemeOptions options) : base(context, scheme, options)
        {

        }
    }
}
