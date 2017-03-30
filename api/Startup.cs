﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchedulePath.Models;
using SchedulePath.Repository;
using SchedulePath.Services;

namespace SchedulePath
{
    public class Startup
    {
        private IConfigurationRoot _config;
        private IHostingEnvironment _env;
        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            // Add framework services.
            services.AddApplicationInsightsTelemetry(_config);

            services.AddDbContext<CepContext>();

            AddScopedServiceMappings(services);

            services.AddMvc();
            services.AddCors();
        }

        private void AddScopedServiceMappings(IServiceCollection services)
        {
            services.AddScoped<ICepRepository, CepRepository>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IActivityProcessor, ActivityProcessor>();
            services.AddScoped<ILinkService, LinkService>();
            services.AddScoped<ILinkProcessor, LinkProcessor>();
            services.AddScoped<IGraphProcessor, GraphProcessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_config.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4200", "http://localhost:8080")
                       .WithMethods("GET", "POST", "PUT", "DELETE")
                       .AllowAnyHeader();
            });

            app.UseMvc();
        }
    }
}