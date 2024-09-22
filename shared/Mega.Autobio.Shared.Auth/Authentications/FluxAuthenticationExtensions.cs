using System;
using Microsoft.AspNetCore.Authentication;

namespace Mega.Autobio.Shared.Auth.Authentications
{
    public static class FluxAuthenticationExtensions
    {
        public static AuthenticationBuilder AddFluxAuth(this AuthenticationBuilder builder, Action<FluxAuthenticationSchemeOptions> configureOptions)
            => builder.AddFluxAuth(FluxAuthenticationDefault.AuthenticationScheme, configureOptions);
        public static AuthenticationBuilder AddFluxAuth(this AuthenticationBuilder builder, string authenticationScheme, Action<FluxAuthenticationSchemeOptions> configureOptions)
            => builder.AddFluxAuth(authenticationScheme, displayName: FluxAuthenticationDefault.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddFluxAuth(this AuthenticationBuilder builder, string authenticationScheme,
            string displayName, Action<FluxAuthenticationSchemeOptions> configureOptions)
        {
            return builder.AddScheme<FluxAuthenticationSchemeOptions, FluxAuthenticationHandler>(authenticationScheme,
                displayName, configureOptions);
        }
    }
}
