using Microsoft.Extensions.DependencyInjection;
using System;

namespace Paynow.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaynow(this IServiceCollection services, Action<PaynowOptions> configureOptions)
        {
            var options = new PaynowOptions();
            configureOptions(options);

            services.AddSingleton(options);

            // Rejestracja HttpClient z implementacją PaynowClient
            services.AddHttpClient<IPaynowClient, PaynowClient>();

            return services;
        }
    }
}