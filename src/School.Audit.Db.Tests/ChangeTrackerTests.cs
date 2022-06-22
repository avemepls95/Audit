using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using School.Audit.AuditConfig;
using School.Audit.Db.Implementation;
using School.Audit.Models;
using Xunit;
using Xunit.Sdk;

namespace School.Audit._Db.Tests
{
    public class ChangeTrackerTests : TestCollection
    {
        [Fact]
        public void IsAnyChanges_ChangesExist_ShouldReturnTrue()
        {
            var fixture = new Fixture();
            var changeTracker = GetInstance(out var dbContext);
            dbContext.Set<SomeClass>().Add(new SomeClass
            {
                IntProperty = fixture.Create<int>(),
                StringProperty = fixture.Create<string>(),
                BoolProperty = fixture.Create<bool>(),
                DateTimeProperty = fixture.Create<DateTimeOffset>()
            });

            changeTracker.IsAnyChanges().Should().BeTrue();
        }
        
        [Fact]
        public void IsAnyChanges_ChangesNotExist_ShouldReturnFalse()
        {
            var fixture = new Fixture();
            var changeTracker = GetInstance(out var dbContext);
            dbContext.Set<AnotherClass>().Add(new AnotherClass
            {
                IntProperty = fixture.Create<int>(),
                StringProperty = fixture.Create<string>(),
                BoolProperty = fixture.Create<bool>(),
                DateTimeProperty = fixture.Create<DateTimeOffset>()
            });

            changeTracker.IsAnyChanges().Should().BeFalse();
        }
        
        [Fact]
        public void GetChanges_NoChanges_ShouldReturnEmptyArray()
        {
            var changeTracker = GetInstance(out var dbContext);
            dbContext.ChangeTracker.AcceptAllChanges();

            var changes = changeTracker.GetChanges();

            changes.Length.Should().Be(0);
        }
        
        [Fact]
        public void GetChanges_NewAuditableEntity_ShouldReturnItemsByEachProperty()
        {
            var fixture = new Fixture();
            var changeTracker = GetInstance(out var dbContext);
            var auditableObject = new SomeClass
            {
                IntProperty = fixture.Create<int>(),
                StringProperty = fixture.Create<string>(),
                BoolProperty = fixture.Create<bool>(),
                ConstantProperty = fixture.Create<bool>(),
                DateTimeProperty = fixture.Create<DateTimeOffset>(),
                EnumProperty = SomeEnum.B,
                NullableDateTimeProperty = null,
                NullableEnumProperty = null,
                AnotherClassObject = null,
            };
            
            dbContext.Set<SomeClass>().Add(auditableObject);

            var changes = changeTracker.GetChanges();
            
            changes.Length.Should().Be(8);
            
            changes
                .All(c =>
                    c.KeyPropertyValue == auditableObject.Id.ToString()
                    && c.OperationType == OperationType.Create
                    && c.TargetType == typeof(SomeClass).ToString()
                    && c.Date != default)
                .Should()
                .BeTrue();
            
            changes.Should().ContainSingle(c =>
                c.NewValue == auditableObject.IntProperty.ToString()
                && c.ChangedPropertyName == nameof(SomeClass.IntProperty));
            
            changes.Should().ContainSingle(c =>
                c.NewValue == auditableObject.StringProperty
                && c.ChangedPropertyName == nameof(SomeClass.StringProperty));
            
            changes.Should().ContainSingle(c =>
                c.NewValue == auditableObject.BoolProperty.ToString()
                && c.ChangedPropertyName == nameof(SomeClass.BoolProperty));
            
            changes.Should().ContainSingle(c =>
                c.NewValue == auditableObject.DateTimeProperty.ToString()
                && c.ChangedPropertyName == nameof(SomeClass.DateTimeProperty));
            
            changes.Should().ContainSingle(c =>
                c.NewValue == auditableObject.EnumProperty.ToString()
                && c.ChangedPropertyName == nameof(SomeClass.EnumProperty));

            var newNullableDateTimePropertyValue = (auditableObject.NullableDateTimeProperty.HasValue
                ? auditableObject.NullableDateTimeProperty.ToString()
                : null);
            changes.Should().ContainSingle(c =>
                c.NewValue == newNullableDateTimePropertyValue
                && c.ChangedPropertyName == nameof(SomeClass.NullableDateTimeProperty));
            
            var newNullableEnumPropertyValue = (auditableObject.NullableDateTimeProperty.HasValue
                ? auditableObject.NullableDateTimeProperty.ToString()
                : null);
            changes.Should().ContainSingle(c =>
                c.NewValue == newNullableEnumPropertyValue
                && c.ChangedPropertyName == nameof(SomeClass.NullableEnumProperty));
        }
        
        [Fact]
        public async Task GetChanges_ChangeAuditableEntity_ShouldReturnItemsByEachProperty()
        {
            var fixture = new Fixture();
            var changeTracker = GetInstance(out var dbContext);
            var savedEntity = new SomeClass
            {
                IntProperty = fixture.Create<int>(),
                StringProperty = fixture.Create<string>(),
                BoolProperty = fixture.Create<bool>(),
                ConstantProperty = fixture.Create<bool>(),
                DateTimeProperty = fixture.Create<DateTimeOffset>(),
                EnumProperty = SomeEnum.B,
                NullableDateTimeProperty = null,
                NullableEnumProperty = null,
                AnotherClassObject = null,
            };
            dbContext.Set<SomeClass>().Add(savedEntity);
            await dbContext.SaveChangesAsync();
            dbContext.Entry(savedEntity).State = EntityState.Detached;

            var modifiedEntity = await dbContext.Set<SomeClass>().FirstAsync(c => c.Id == savedEntity.Id);
            modifiedEntity.IntProperty = fixture.Create<int>();
            modifiedEntity.StringProperty = fixture.Create<string>();
            modifiedEntity.BoolProperty = !savedEntity.BoolProperty;
            modifiedEntity.DateTimeProperty = fixture.Create<DateTimeOffset>();
            modifiedEntity.EnumProperty = SomeEnum.A;
            modifiedEntity.NullableDateTimeProperty = fixture.Create<DateTimeOffset>();
            modifiedEntity.NullableEnumProperty = SomeEnum.A;

            dbContext.Set<SomeClass>().Update(modifiedEntity);

            var changes = changeTracker.GetChanges();
            
            changes.Length.Should().Be(7);
            
            changes
                .All(c =>
                    c.KeyPropertyValue == savedEntity.Id.ToString()
                    && c.OperationType == OperationType.Modify
                    && c.TargetType == typeof(SomeClass).ToString()
                    && c.Date != default)
                .Should()
                .BeTrue();
            
            changes.Should().ContainSingle(c =>
                c.OldValue == savedEntity.IntProperty.ToString()
                & c.NewValue == modifiedEntity.IntProperty.ToString()
                && c.ChangedPropertyName == nameof(SomeClass.IntProperty));
            
            changes.Should().ContainSingle(c =>
                c.OldValue == savedEntity.StringProperty
                & c.NewValue == modifiedEntity.StringProperty
                && c.ChangedPropertyName == nameof(SomeClass.StringProperty));
            
            changes.Should().ContainSingle(c =>
                c.OldValue == savedEntity.BoolProperty.ToString()
                & c.NewValue == modifiedEntity.BoolProperty.ToString()
                && c.ChangedPropertyName == nameof(SomeClass.BoolProperty));
            
            changes.Should().ContainSingle(c =>
                c.OldValue == savedEntity.DateTimeProperty.ToString()
                & c.NewValue == modifiedEntity.DateTimeProperty.ToString()
                && c.ChangedPropertyName == nameof(SomeClass.DateTimeProperty));
            
            changes.Should().ContainSingle(c =>
                c.OldValue == savedEntity.EnumProperty.ToString()
                & c.NewValue == modifiedEntity.EnumProperty.ToString()
                && c.ChangedPropertyName == nameof(SomeClass.EnumProperty));
            
            var oldNullableDateTimePropertyValue = (savedEntity.NullableDateTimeProperty.HasValue
                ? savedEntity.NullableDateTimeProperty.ToString()
                : null);
            var newNullableDateTimePropertyValue = (modifiedEntity.NullableDateTimeProperty.HasValue
                ? modifiedEntity.NullableDateTimeProperty.ToString()
                : null);
            changes.Should().ContainSingle(c =>
                c.OldValue == oldNullableDateTimePropertyValue
                & c.NewValue == newNullableDateTimePropertyValue
                && c.ChangedPropertyName == nameof(SomeClass.NullableDateTimeProperty));
            
            var oldNullableEnumPropertyValue = (savedEntity.NullableEnumProperty.HasValue
                ? savedEntity.NullableEnumProperty.ToString()
                : null);
            var newNullableEnumPropertyValue = (modifiedEntity.NullableEnumProperty.HasValue
                ? modifiedEntity.NullableEnumProperty.ToString()
                : null);
            changes.Should().ContainSingle(c =>
                c.OldValue == oldNullableEnumPropertyValue
                & c.NewValue == newNullableEnumPropertyValue
                && c.ChangedPropertyName == nameof(SomeClass.NullableEnumProperty));
        }
        
        [Fact]
        public async Task GetChanges_DeleteAuditableEntity_ShouldReturnOneItem()
        {
            var fixture = new Fixture();
            var changeTracker = GetInstance(out var dbContext);
            var savedEntity = new SomeClass
            {
                IntProperty = fixture.Create<int>(),
                StringProperty = fixture.Create<string>(),
                BoolProperty = fixture.Create<bool>(),
                DateTimeProperty = fixture.Create<DateTimeOffset>(),
                EnumProperty = SomeEnum.B,
                NullableDateTimeProperty = null,
                NullableEnumProperty = null,
                AnotherClassObject = null,
            };
            dbContext.Set<SomeClass>().Add(savedEntity);
            await dbContext.SaveChangesAsync();
            dbContext.Entry(savedEntity).State = EntityState.Detached;

            var modifiedEntity = await dbContext.Set<SomeClass>().FirstAsync(c => c.Id == savedEntity.Id);
            dbContext.Set<SomeClass>().Remove(modifiedEntity);

            var changes = changeTracker.GetChanges();
            
            changes.Length.Should().Be(1);
            
            var item = changes[0];
            item.Should().Match<AuditItem>(i => 
                i.OperationType == OperationType.Delete
                && i.NewValue == null
                && i.OldValue == null
                && i.TargetType == typeof(SomeClass).ToString()
                && i.ChangedPropertyName == null
                && i.KeyPropertyValue == savedEntity.Id.ToString()
                && i.Date != default
            );
        }

        private ChangesProvider<DbContext> GetInstance(out TestDbContext dbContext)
        {
            // Configure types
            var typesBuilder = new AuditableTypesBuilder();
            typesBuilder
                .Add<SomeClass>(nameof(SomeClass.Id))
                .AddProperties(
                    nameof(SomeClass.BoolProperty),
                    nameof(SomeClass.ConstantProperty),
                    nameof(SomeClass.IntProperty),
                    nameof(SomeClass.StringProperty),
                    nameof(SomeClass.DateTimeProperty),
                    nameof(SomeClass.EnumProperty),
                    nameof(SomeClass.NullableEnumProperty),
                    nameof(SomeClass.NullableDateTimeProperty));
            
            // Configure db
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(new SqliteConnection("DataSource=:memory:"));
            dbContext = new TestDbContext(optionsBuilder.Options);
            dbContext.Database.OpenConnection();
            dbContext.Database.EnsureCreated();
            
            var instance = new ChangesProvider<DbContext>(dbContext, typesBuilder.Types);

            return instance;
        }
    }
}