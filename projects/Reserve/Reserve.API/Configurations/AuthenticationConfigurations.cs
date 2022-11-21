using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Reserve.API.Configurations
{
    public static class AuthenticationConfigurations
    {
        public static void AddJWTAuthentication(this IServiceCollection services, IConfiguration configured)
        {
            var configurations = configured.GetSection("Authentication");
            
            var secret = configurations
                .GetSection("JWTSecret")
                .Value;

            var key = Encoding.UTF8.GetBytes(secret);
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }
    }
}