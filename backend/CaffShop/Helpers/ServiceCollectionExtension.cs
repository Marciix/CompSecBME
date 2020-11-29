using System;
using System.Threading.Tasks;
using CaffShop.Models.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CaffShop.Helpers
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddJwtMiddleware(this IServiceCollection services,
            Func<TokenValidatedContext, Task> onTokenValidated = null)
        {
            var serviceProvider = services.BuildServiceProvider();
            var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

            return AddJwtMiddleware(services, jwtOptions, onTokenValidated);
        }

        public static IServiceCollection AddJwtMiddleware(this IServiceCollection services, JwtOptions jwtOptions,
            Func<TokenValidatedContext, Task> onTokenValidated = null)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = GetParameters(jwtOptions);
                if (onTokenValidated != null)
                {
                    jwtBearerOptions.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = onTokenValidated
                    };
                }
            });
            return services;
        }

        private static TokenValidationParameters GetParameters(JwtOptions jwtOptions)
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtOptions.SecretBytes)
            };

            if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
            {
                parameters.ValidateIssuer = false;
            }
            else
            {
                parameters.ValidateIssuer = true;
                parameters.ValidIssuer = jwtOptions.Issuer;
            }

            if (jwtOptions.Audiences.Length == 0)
            {
                parameters.ValidateAudience = false;
            }
            else
            {
                parameters.ValidateAudience = true;
                parameters.ValidAudiences = jwtOptions.Audiences;
            }

            return parameters;
        }
    }
}