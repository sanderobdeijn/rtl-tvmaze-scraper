using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Rtl.TvMaze.Data;
using Rtl.TvMaze.Scraper;
using Rtl.TvMaze.Shows;

namespace Rtl.TvMaze
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TvMazeDbContext>(options =>
                options
                    .UseSqlServer(this.configuration.GetConnectionString("DefaultConnection"))
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
            );

            services.Configure<TvMazeOptions>(configuration.GetSection("TvMazeOptions"));

            services.AddTransient<IScraperNode, ScraperNode>();
            services.AddTransient<IShowsService, ShowsService>();

            services.AddHostedService<ScraperService>();

            var registry = services.AddPolicyRegistry();

            var defaultPolicy = Policy
                .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.TooManyRequests)
                .OrTransientHttpError()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(5));

            registry.Add("Default", defaultPolicy);

            services.AddHttpClient<TvMazeScraperHttpClient>()
                .AddPolicyHandlerFromRegistry("Default");

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TvMazeDbContext tvMazeContext)
        {
            try
            {
                tvMazeContext.Database.Migrate();
                Console.WriteLine("Database migrated.");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rtl TvMaze API V1");
                c.RoutePrefix = string.Empty;
             });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
