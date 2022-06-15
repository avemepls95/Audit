using System;
using School.Audit.Abstractions;

namespace School.Audit._Db.Tests
{
    public class SomeClass
    {
        public Guid Id { get; } = Guid.NewGuid();
        
        public int IntProperty { get; set; }
        
        public string StringProperty { get; set; }
        
        public bool BoolProperty { get; set; }
        
        public DateTimeOffset DateTimeProperty { get; set; }
    }
}