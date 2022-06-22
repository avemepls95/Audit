using System;

namespace School.SandBox.Models
{
    public class SomeClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public int IntProperty { get; set; }
        
        public string StringProperty { get; set; }
        
        public bool BoolProperty { get; set; }
        
        public DateTimeOffset DateTimeProperty { get; set; }

        public SomeEnum EnumProperty { get; set; }
        
        public DateTimeOffset? NullableDateTimeProperty { get; set; }

        public SomeEnum? NullableEnumProperty { get; set; }
        
        public AnotherClass AnotherClassObject { get; set; }
        
        // public SomeStruct SomeStructObject { get; set; }

        public int GetInt()
        {
            return IntProperty;
        }
    }
}