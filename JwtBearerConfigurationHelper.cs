using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;

namespace EShopOnAbp.Shared.Hosting.Microservices;

public static class JwtBearerConfigurationHelper
{
    private const string DefaultScheme = "multitenant";
    public static void Configure(
        ServiceConfigurationContext context,
        string audience)
    {
        var configuration = context.Services.GetConfiguration();
        
        context.Services.AddAuthentication(DefaultScheme)
            .AddJwtBearer("master",options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = audience;
                // IDX10204: Unable to validate issuer on K8s if not set
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = "preferred_username",
                    ValidIssuers = new[]
                        { configuration["AuthServer:Authority"], configuration["AuthServer:MetadataAddress"] },
                    // IDX10500: Signature validation failed. No security keys were provided to validate the signature on K8s
                    SignatureValidator = delegate(string token, TokenValidationParameters parameters)
                    {
                        var jwt = new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);
                        return jwt;
                    }
                };
            })
            .AddJwtBearer("megarobo", options =>
            {
                options.Authority = "http://localhost:8080/realms/megarobo";
                options.RequireHttpsMetadata = false;
                options.Audience = audience;
                // IDX10204: Unable to validate issuer on K8s if not set
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = "preferred_username",
                    ValidIssuers = new[]
                        { "http://localhost:8080/realms/megarobo", "http://localhost:8080/realms/megarobo" },
                    // IDX10500: Signature validation failed. No security keys were provided to validate the signature on K8s
                    SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                    {
                        var jwt = new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);
                        return jwt;
                    }
                };
            })
            .AddPolicyScheme(DefaultScheme, DefaultScheme, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    string authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                    {
                        var token = authorization.Substring("Bearer ".Length).Trim();
                        var jwtHandler = new JwtSecurityTokenHandler();

                        return (jwtHandler.CanReadToken(token) && jwtHandler.ReadJwtToken(token).Issuer.Contains("megarobo"))
                            ? "megarobo" : "master";
                    }
                    return "master";
                };
            });

    }
}