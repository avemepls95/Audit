using System;

namespace School.Audit._Db.Tests
{
    public class SomeClass
    {
        public Guid Id { get; } = Guid.NewGuid();
        
        public int IntProperty { get; set; }
        
        public string StringProperty { get; set; }
        
        public bool BoolProperty { get; set; }
        
        public bool ConstantProperty { get; set; }
        
        public DateTimeOffset DateTimeProperty { get; set; }
        
        public SomeEnum EnumProperty { get; set; }
        
        public DateTimeOffset? NullableDateTimeProperty { get; set; }

        public SomeEnum? NullableEnumProperty { get; set; }
        
        public AnotherClass AnotherClassObject { get; set; }
    }
}