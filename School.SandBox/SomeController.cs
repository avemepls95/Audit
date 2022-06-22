using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School.SandBox.Models;

namespace School.SandBox
{
    public class SomeController : ControllerBase
    {
        private readonly MyDbContext _dbContext;

        public SomeController(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost, Route("save")]
        public async Task Save(CancellationToken cancellationToken)
        {
            var someObject = new SomeClass
            {
                IntProperty = 1,
                StringProperty = "test",
                BoolProperty = false,
                DateTimeProperty = DateTimeOffset.Now,
                EnumProperty = SomeEnum.B
            };
            
            var anotherObject = new AnotherClass()
            {
                Lol = 1,
            };
            
            await _dbContext.Set<SomeClass>().AddAsync(someObject, cancellationToken);
            await _dbContext.Set<AnotherClass>().AddAsync(anotherObject, cancellationToken);
        }
        
        [HttpPost, Route("test")]
        public async Task Test(CancellationToken cancellationToken)
        {
            var someClass = await _dbContext.Set<SomeClass>()
                .FirstAsync(c => c.Id == Guid.Parse("d816694f-7cd9-4c68-b1dd-c27fd63b33fe"), cancellationToken);
            someClass.StringProperty = "123";
            _dbContext.Set<SomeClass>().Update(someClass);
            
            var anotherClass = await _dbContext.Set<AnotherClass>()
                .FirstAsync(c => c.Id == Guid.Parse("a3bba03d-7470-41ed-b57a-753d17d25fda"), cancellationToken);
            _dbContext.Set<AnotherClass>().Remove(anotherClass);
        }
    }
}