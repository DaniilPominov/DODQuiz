using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Application.Abstract.Security;
using DODQuiz.Infrastructure.Data.Context;
using DODQuiz.Infrastructure.Data.Repos;
using DODQuiz.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
namespace DODQuiz.Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IJWTProvider, JWTProvider>();

            return services;
        }
        public static IServiceCollection AddPersistance(this IServiceCollection services)
        {
            services.AddDbContext<GameContext>();

            services.AddScoped<IPlayerRepos, PlayerRepos>();
            services.AddScoped<IQuestionRepos, QuestionRepos>();

            return services;
        }

    }
}
