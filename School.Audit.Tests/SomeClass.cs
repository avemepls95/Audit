using System;
using School.Audit.Abstractions;

namespace School.Audit.Tests
{
    public class SomeClass : IAuditable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int IntType { get; set; }
        public string StringProperty { get; set; }
        public bool BoolProperty { get; set; }
        public DateTimeOffset DateTimeProperty { get; set; }
    }
}