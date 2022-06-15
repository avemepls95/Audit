using System;

namespace School.SandBox
{
    public class SomeClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int IntType { get; set; }
        public string StringProperty { get; set; }
        public bool BoolProperty { get; set; }
        public DateTimeOffset DateTimeProperty { get; set; }
    }
}