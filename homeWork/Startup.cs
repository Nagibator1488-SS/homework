using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using homeWork.Extensions;
using homeWork.Infrastructure;
using homeWork.Infrastructure.Repositories;
using MongoDB.Driver;

namespace homeWork
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.RegisterDependencies();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Будет падать пока БД не захостирована
            // EnsureBankIntegrationsCollectionExists();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void EnsureBankIntegrationsCollectionExists()
        {
            var mongoClient = new MongoClient(DBSettings.ConnectionString);
            var repository = new ProductRepository(mongoClient, Core.Logging.Logging.GetLog());
            repository.EnsureCollectionExists();
        }
    }
}
