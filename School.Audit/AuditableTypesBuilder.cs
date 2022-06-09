using System;
using System.Linq;
using System.Reflection;

namespace School.Audit
{
    internal class AuditableTypesBuilder : IAuditableTypesBuilder
    {
        public AuditableTypes Types { get; set; }
        
        public IAuditableTypesBuilder Add<T>(params Func<T, object>[] getPropertyFuncs) where T : IAuditable
        {
            var type = typeof(T);
            if (Types.Contains(type))
            {
                return this;
            }

            var propertyNames = getPropertyFuncs
                .Select(f => f.GetMethodInfo().ReturnParameter.Name!.ToString())
                .ToArray();

            Types.Add(type, propertyNames);
            
            return this;
        }
    }
}