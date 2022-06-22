using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using School.Audit.Db;
using School.SandBox.Models;

namespace School.SandBox
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            var contextConnectionString = _configuration.GetConnectionString("DbConnection");
            services.AddDbAudit<MyDbContext>(builder =>
            {
                builder
                    .Add<SomeClass>(c => c.Id)
                    .AddAllProperties();
                
                builder
                    .Add<AnotherClass>(nameof(AnotherClass.Id))
                    .AddProperties(
                        c => c.Lol,
                        c => c.Kek);
            });
            
            services.AddDbContext(contextConnectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}