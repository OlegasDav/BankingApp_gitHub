using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Respositories;
using System;

namespace Persistence
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            SqlMapper.AddTypeHandler(new MySqlGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            return services
                .AddRepositories()
                .AddSqlClient(configuration);
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services
                .AddSingleton<IUsersRepository, UsersRepository>()
                .AddSingleton<IAccountsRepository, AccountsRepository>()
                .AddSingleton<ITransactionsRepository, TransactionsRepository>();

            return services;
        }

        private static IServiceCollection AddSqlClient(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlConnectionString");

            return services.AddTransient<ISqlClient>(_ => new SqlClient(connectionString));
        }
    }
}
