using System;
using School.Audit;

namespace School.SandBox
{
    public class SomeClass : IAuditable
    {
        public int IntType { get; set; }
        public string StringProperty { get; set; }
        public bool BoolProperty { get; set; }
        public DateTimeOffset DateTimeProperty { get; set; }
    }
}