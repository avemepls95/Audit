using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace School.SandBox
{
    public class SomeController : ControllerBase
    {
        private readonly DbContext _dbContext;

        public SomeController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost, Route("save")]
        public async Task Save(CancellationToken cancellationToken)
        {
            var someObject = new SomeClass
            {
                IntType = 1,
                StringProperty = "test",
                BoolProperty = false,
                DateTimeProperty = DateTimeOffset.Now
            };

            _dbContext.Set<SomeClass>().Add(someObject);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}