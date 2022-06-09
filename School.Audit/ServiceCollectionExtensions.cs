using System;
using Microsoft.Extensions.DependencyInjection;

namespace School.Audit
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAudit(
            this IServiceCollection serviceCollection,
            Action<IAuditableTypesBuilder> configureOptions)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var builder = new AuditableTypesBuilder();
            configureOptions.Invoke(builder);

            serviceCollection.AddSingleton(builder.Types);

            return serviceCollection;
        }
    }  
}

