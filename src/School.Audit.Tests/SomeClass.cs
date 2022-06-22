using System;

namespace School.Audit.Tests
{
    public class SomeClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public int IntProperty { get; set; }
        
        public string StringProperty { get; set; }
        
        public bool BoolProperty { get; set; }
        
        public DateTimeOffset DateTimeProperty { get; set; }

        public AnotherClass AnotherClassObject { get; set; }
    }
}