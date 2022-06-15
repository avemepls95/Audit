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
                DateTimeProperty = fixture.Create<DateTimeOffset>()
            };
            dbContext.Set<SomeClass>().Add(auditableObject);

            var changes = changeTracker.GetChanges();
            
            changes.Length.Should().Be(4);
            
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
                DateTimeProperty = fixture.Create<DateTimeOffset>()
            };
            dbContext.Set<SomeClass>().Add(savedEntity);
            await dbContext.SaveChangesAsync();
            dbContext.Entry(savedEntity).State = EntityState.Detached;

            var modifiedEntity = await dbContext.Set<SomeClass>().FirstAsync(c => c.Id == savedEntity.Id);
            modifiedEntity.IntProperty = fixture.Create<int>();
            modifiedEntity.StringProperty = fixture.Create<string>();
            modifiedEntity.BoolProperty = fixture.Create<bool>();
            modifiedEntity.DateTimeProperty = fixture.Create<DateTimeOffset>();

            dbContext.Set<SomeClass>().Update(modifiedEntity);

            var changes = changeTracker.GetChanges();
            
            changes.Length.Should().Be(4);
            
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
                DateTimeProperty = fixture.Create<DateTimeOffset>()
            };
            dbContext.Set<SomeClass>().Add(savedEntity);
            await dbContext.SaveChangesAsync();
            dbContext.Entry(savedEntity).State = EntityState.Detached;

            var modifiedEntity = await dbContext.Set<SomeClass>().FirstAsync(c => c.Id == savedEntity.Id);
            dbContext.Set<SomeClass>().Remove(modifiedEntity);

            var changes = changeTracker.GetChanges();
            
            changes.Length.Should().Be(1);
            
            var item = changes[0];
            item.OperationType.Should().Be(OperationType.Delete);
            item.NewValue.Should().BeNull();
            item.OldValue.Should().BeNull();
            item.TargetType.Should().Be(typeof(SomeClass).ToString());
            item.ChangedPropertyName.Should().BeNull();
            item.KeyPropertyValue.Should().Be(savedEntity.Id.ToString());
            item.Date.Should().NotBe(default);
        }

        private ChangeTracker<DbContext> GetInstance(out TestDbContext dbContext)
        {
            // Configure types
            var typesBuilder = new AuditableTypesBuilder();
            typesBuilder
                .Add<SomeClass>(nameof(SomeClass.Id))
                .AddProperties(
                    nameof(SomeClass.BoolProperty),
                    nameof(SomeClass.IntProperty),
                    nameof(SomeClass.StringProperty),
                    nameof(SomeClass.DateTimeProperty));
            
            // Configure db
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(new SqliteConnection("DataSource=:memory:"));
            dbContext = new TestDbContext(optionsBuilder.Options);
            dbContext.Database.OpenConnection();
            dbContext.Database.EnsureCreated();
            
            var instance = new ChangeTracker<DbContext>(dbContext, typesBuilder.Types);

            return instance;
        }
    }
}