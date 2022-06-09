using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace School.SandBox
{
    public static class Config
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
        {
            var contentDatabaseOptions = new DbContextOptionsBuilder<DbContext>()
                .UseLoggerFactory(MyLoggerFactory)
                .UseNpgsql(connectionString).Options;
            services.AddSingleton(contentDatabaseOptions);

            return services;
        }
    }
}
