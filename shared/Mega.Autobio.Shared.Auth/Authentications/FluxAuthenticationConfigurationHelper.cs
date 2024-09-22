using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mega.Autobio.Shared.Auth.Authentications;

public class FluxAuthenticationConfigurationHelper
{
    public static void Configure(
        IServiceCollection service,IConfiguration configuration)
    {
        Action<FluxAuthenticationSchemeOptions> optionsAction = options =>
        {
            options.Issuer = configuration["AuthServer:Authority"]!;
            options.IssuerJwksUri = configuration["AuthServer:AuthorityJwks"]!;
        };
        service.Configure<FluxAuthenticationSchemeOptions>(optionsAction);

        service
            .AddAuthentication(FluxAuthenticationDefault.AuthenticationScheme)
            .AddFluxAuth(optionsAction);
    }
}