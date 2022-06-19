using System;

namespace School.Audit.AuditConfig
{
    public class AuditableEntityMetaData : IEquatable<AuditableEntityMetaData>
    {
        public Type Type { get; init; }
        
        public string KeyPropertyName { get; set; }

        public string[] PropertyNames { get; set; } = Array.Empty<string>();

        public bool Equals(AuditableEntityMetaData other)
        {
            return other != null && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is AuditableEntityMetaData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);
        }

        public static bool operator ==(AuditableEntityMetaData left, AuditableEntityMetaData right)
        {
            return left != null && left.Equals(right);
        }

        public static bool operator !=(AuditableEntityMetaData left, AuditableEntityMetaData right)
        {
            return !(left == right);
        }
    }
}