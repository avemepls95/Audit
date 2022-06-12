﻿using System;
using FluentAssertions;
using School.Audit.AuditConfig;
using Xunit;
using Xunit.Sdk;

namespace School.Audit.Tests
{
    public class AuditableTypesBuilderTests : TestCollection
    {
        [Fact]
        public void Add_ValidData_Ok()
        {
            var builder = new AuditableTypesBuilder();
            builder.Add<SomeClass>(nameof(SomeClass.Id));

            builder.Types.Contains(typeof(SomeClass)).Should().BeTrue();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Add_InvalidKeyPropertyName_ShouldThrow(string invalidKeyPropertyName)
        {
            var builder = new AuditableTypesBuilder();
            
            Action func = () => builder.Add<SomeClass>(invalidKeyPropertyName);
            func.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void Add_DuplicateAuditableType_ShouldThrow()
        {
            var builder = new AuditableTypesBuilder();
            
            builder.Add<SomeClass>(nameof(SomeClass.Id));
            Action func = () => builder.Add<SomeClass>(nameof(SomeClass.Id));
            
            func.Should()
                .Throw<ArgumentException>()
                .Where(e => e.Message.Contains(typeof(SomeClass).ToString()));
        }
    }
}