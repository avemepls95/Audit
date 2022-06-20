using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using School.Audit.Abstractions;
using School.Audit.AuditConfig.Abstractions;
using School.Audit.Db.Implementation;

namespace School.Audit.Db
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbAudit<TDbContext>(
            this IServiceCollection serviceCollection,
            Action<IAuditableTypesBuilder> buildAuditableTypes) where TDbContext : DbContext 
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (buildAuditableTypes == null)
            {
                throw new ArgumentNullException(nameof(buildAuditableTypes));
            }

            serviceCollection.AddAudit(buildAuditableTypes);

            serviceCollection.AddScoped<ISaveChangesCommand, SaveChangesCommand<TDbContext>>();
            serviceCollection.AddScoped<IChangesProvider, ChangesProvider<TDbContext>>();
            
            return serviceCollection;
        }
    }  
}

