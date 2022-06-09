using System;

namespace School.Audit
{
    public sealed class AuditItem<TId> where TId : struct
    {
        public Type TargetType { get; set; }

        public TId Id { get; set; }

        public string ChangedPropertyName { get; set; }

        public object OldValue { get; set; }
    
        public object NewValue { get; set; }

        public OperationType OperationType { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
