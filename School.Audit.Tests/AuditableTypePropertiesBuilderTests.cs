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
                .Where(e => e.Message.Contains("Duplicate"));
        }

        [Fact]
        public void AddProperties_AddIdProperty_ShouldNotAdd()
        {
            var typesBuilder = new AuditableTypesBuilder();
            typesBuilder
                .Add<SomeClass>(nameof(SomeClass.Id))
                .AddProperties(nameof(SomeClass.Id));

            var propertyNames = typesBuilder.Types.Get<SomeClass>().PropertyNames;
            propertyNames.Length.Should().Be(0);
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
                .Where(e => e.Message.Contains("supported"));
        }
    }
}