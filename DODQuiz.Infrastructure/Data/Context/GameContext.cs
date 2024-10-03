using DODQuiz.Core.Entyties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DODQuiz.Infrastructure.Data.Context
{
    public class GameContext : DbContext
    {
        protected readonly IConfiguration? _configuration;

        public GameContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public GameContext() { }
        public DbSet<User> users { get; set; }
        public DbSet<Question> questions { get; set; }
        public DbSet<Role> roles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var options = _configuration?.GetRequiredSection("ConnectionStrings");
            optionsBuilder.UseSqlite(options?.GetRequiredSection("QuizApp").Value ?? "Data Source=.\\Data\\QuizAppDatabase.db")
                .UseLoggerFactory(CreateLoggerFactory())
                .EnableSensitiveDataLogging();
        }
        public ILoggerFactory CreateLoggerFactory() => LoggerFactory
            .Create(builder => { builder.AddConsole(); });
    }
}
