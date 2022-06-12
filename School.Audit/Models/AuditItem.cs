using System;

namespace School.Audit.Models
{
    public sealed class AuditItem
    {
        public int Id { get; set; }
        
        public string TargetType { get; set; }

        public string KeyPropertyValue { get; set; }

        public string ChangedPropertyName { get; set; }

        public string OldValue { get; set; }
    
        public string NewValue { get; set; }

        public OperationType OperationType { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
