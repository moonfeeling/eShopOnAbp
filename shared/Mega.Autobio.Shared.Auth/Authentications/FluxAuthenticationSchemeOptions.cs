using Microsoft.AspNetCore.Authentication;

namespace Mega.Autobio.Shared.Auth.Authentications;

public class FluxAuthenticationSchemeOptions: AuthenticationSchemeOptions
{
    /// <summary>
    /// 认证服务地址
    /// </summary>
    public string Issuer { get; set; }
    /// <summary>
    /// 认证服务的签名密钥集合地址
    /// </summary>
    public string IssuerJwksUri { get; set; }
}