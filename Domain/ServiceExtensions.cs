using Domain.Clients.Firebase;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            return services
                .AddClients()
                .AddServices();
        }

        private static IServiceCollection AddClients(this IServiceCollection services)
        {
            services.AddHttpClient<IFirebaseClient, FirebaseClient>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<IAccountService, AccountService>()
                .AddSingleton<ITransferService, TransferService>();

            return services;
        }
    }
}
