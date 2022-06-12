﻿using System;
using Microsoft.Extensions.DependencyInjection;
using School.Audit.Abstractions;
using School.Audit.AuditConfig;

namespace School.Audit
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAudit(
            this IServiceCollection serviceCollection,
            Action<IAuditableTypesBuilder> buildAuditableTypes)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (buildAuditableTypes == null)
            {
                throw new ArgumentNullException(nameof(buildAuditableTypes));
            }

            var builder = new AuditableTypesBuilder();
            buildAuditableTypes.Invoke(builder);

            serviceCollection.AddSingleton(builder.Types);
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

            return serviceCollection;
        }
    }  
}

