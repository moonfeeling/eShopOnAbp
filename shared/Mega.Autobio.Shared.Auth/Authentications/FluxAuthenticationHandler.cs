using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Volo.Abp.Security.Claims;

namespace Mega.Autobio.Shared.Auth.Authentications
{
    public class FluxAuthenticationHandler : AuthenticationHandler<FluxAuthenticationSchemeOptions>
    {
        private readonly FluxAuthenticationSchemeOptions FluxAuthenticationSchemeOptions;
        public FluxAuthenticationHandler(IOptionsMonitor<FluxAuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            FluxAuthenticationSchemeOptions = options.CurrentValue;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorizationHeader = Request.Headers[AuthenticationConstants.AuthorizationHeader];
            
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }
            
            var token = string.Empty;
            if (authorizationHeader.StartsWith(FluxAuthenticationDefault.AuthenticationScheme, StringComparison.Ordinal))
            {
                token = authorizationHeader.Substring(FluxAuthenticationDefault.AuthenticationScheme.Length).Trim();
            }
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            var client = new HttpClient();
            var jwks = await client.GetStringAsync(FluxAuthenticationSchemeOptions.IssuerJwksUri);
            //注意：使用第二个签名密钥
            var key = new JsonWebKeySet(jwks).Keys[1];
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                NameClaimType = "preferred_username",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = FluxAuthenticationSchemeOptions.Issuer,
                //不验证受众
                ValidateAudience =false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero  // 禁止时间差
            };
            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch (Exception ex)
            {
                // Token 验证失败
                return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
            }

            var validatedContext = new FluxTokenValidatedContext(Context, Scheme, Options)
            {
                Principal = principal
            };

            validatedContext.Success();

            return validatedContext.Result;
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();

            Response.Headers.Add(HeaderNames.WWWAuthenticate, FluxAuthenticationDefault.AuthenticationScheme);
            Response.StatusCode = 401;
            if (authResult.Failure != null && !string.IsNullOrEmpty(authResult.Failure.Message))
            {
                var byteMsg = System.Text.Encoding.Default.GetBytes(authResult.Failure.Message);
                await Response.Body.WriteAsync(byteMsg, 0, byteMsg.Length);
            }
        }
    }
}
