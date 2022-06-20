using System;
using System.Linq;
using FluentAssertions;
using School.Audit.AuditConfig;
using Xunit;
using Xunit.Sdk;

namespace School.Audit.Tests
{
    public class AuditableTypePropertiesBuilderTests : TestCollection
    {
        [Fact]
        public void AddProperties_ValidData_Ok()
        {
            var typesBuilder = new AuditableTypesBuilder();
            typesBuilder
                .Add<SomeClass>(nameof(SomeClass.Id))
                .AddProperties(
                    nameof(SomeClass.BoolProperty),
                    nameof(SomeClass.StringProperty),
                    nameof(SomeClass.IntProperty),
                    nameof(SomeClass.DateTimeProperty));

            var propertyNames = typesBuilder.Types.Get<SomeClass>().PropertyNames;
            propertyNames.Length.Should().Be(4);
            propertyNames.Contains(nameof(SomeClass.BoolProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.StringProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.BoolProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.IntProperty)).Should().BeTrue();
        }
        
        [Fact]
        public void AddProperties_AddSamePropertyTwoTimes_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperties(
                nameof(SomeClass.BoolProperty),
                nameof(SomeClass.BoolProperty));

            func.Should()
                .Throw<ArgumentException>()
                .Where(e => e.Message.Contains("Duplicate", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void AddProperties_AddIdProperty_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperties(nameof(SomeClass.Id));
            
            func.Should()
                .Throw<ArgumentException>()
                .Where(e => e.Message.Contains("Key", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void AddProperties_AddNotExistProperty_ShouldThrow()
        {
            var notExistPropertyName = Guid.NewGuid().ToString();
            
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperties(notExistPropertyName);

            func.Should()
                .Throw<ArgumentException>()
                .Where(e => e.Message.Contains(notExistPropertyName));
        }

        [Fact]
        public void AddProperties_AddNotAllowedProperty_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperties(nameof(SomeClass.AnotherClassObject));

            func.Should()
                .Throw<ArgumentException>()
                .Where(e => e.Message.Contains("invalid type", StringComparison.OrdinalIgnoreCase));
        }
        
        [Fact]
        public void AddProperties_Generic_ValidData_Ok()
        {
            var typesBuilder = new AuditableTypesBuilder();
            typesBuilder
                .Add<SomeClass>(nameof(SomeClass.Id))
                .AddProperties(
                    c => c.BoolProperty,
                    c => c.IntProperty,
                    c => c.StringProperty,
                    c => c.DateTimeProperty);

            var propertyNames = typesBuilder.Types.Get<SomeClass>().PropertyNames;
            propertyNames.Length.Should().Be(4);
            propertyNames.Contains(nameof(SomeClass.BoolProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.StringProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.BoolProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.IntProperty)).Should().BeTrue();
        }
        
        [Fact]
        public void AddProperties_Generic_AddSamePropertyTwoTimes_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperties(
                c => c.BoolProperty,
                c => c.BoolProperty);

            func.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void AddProperties_Generic_AddNotProperty_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperties(c => c.GetType());

            func.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void AddProperties_Generic_AddIdProperty_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperties(c => c.Id);

            func.Should().Throw<ArgumentException>().Where(e => e.Message.Contains("Key", StringComparison.OrdinalIgnoreCase));
        }
        
        [Fact]
        public void AddProperty_Generic_ValidData_Ok()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            typePropertiesBuilder.AddProperty(c => c.IntProperty);

            var propertyNames = typesBuilder.Types.Get<SomeClass>().PropertyNames;
            propertyNames.Length.Should().Be(1);
            propertyNames.Contains(nameof(SomeClass.IntProperty)).Should().BeTrue();
        }
        
        [Fact]
        public void AddProperty_Generic_AddSamePropertyTwoTimes_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            typePropertiesBuilder.AddProperty(c => c.IntProperty);
            
            Action func = () => typePropertiesBuilder.AddProperty(c => c.IntProperty);
            func.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void AddProperty_Generic_AddNotProperty_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperty(c => c.GetType());

            func.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void AddProperty_Generic_AddIdProperty_ShouldThrow()
        {
            var typesBuilder = new AuditableTypesBuilder();
            var typePropertiesBuilder = typesBuilder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => typePropertiesBuilder.AddProperty(c => c.Id);

            func.Should().Throw<ArgumentException>().Where(e => e.Message.Contains("Key", StringComparison.OrdinalIgnoreCase));
        }
        
        [Fact]
        public void AddAllProperties_ValidData_Ok()
        {
            var typesBuilder = new AuditableTypesBuilder();
            typesBuilder
                .Add<SomeClass>(nameof(SomeClass.Id))
                .AddAllProperties();

            var propertyNames = typesBuilder.Types.Get<SomeClass>().PropertyNames;
            propertyNames.Length.Should().Be(4);
            propertyNames.Contains(nameof(SomeClass.BoolProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.StringProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.BoolProperty)).Should().BeTrue();
            propertyNames.Contains(nameof(SomeClass.IntProperty)).Should().BeTrue();
        }
    }
}