using DODQuiz.Application.Abstract.Services;
using DODQuiz.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DODQuiz.API.Extensions
{
    public static class ApiExtensions
    {
        public static void AddQuizGame(this IServiceCollection services)
        {
            
            services.AddTransient<IProfileService, ProfileService>();
            services.AddSingleton<IGameService, GameService>();
        }
        public static void AddApiAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtoptions = configuration.GetRequiredSection("jwtoptions");
            var secretKey = jwtoptions.GetRequiredSection("SecretKey").Value;
            var key = Encoding.UTF8.GetBytes(secretKey);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["bivis-bober"];

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(

                options =>
                {
                    options.AddPolicy("admin",
                        policy =>
                        {
                            policy.RequireRole("admin");

                        });

                    options.AddPolicy("user",
                        policy =>
                        {
                            policy.RequireRole("admin", "user");

                        });
                }
             );
        }
    }
}
