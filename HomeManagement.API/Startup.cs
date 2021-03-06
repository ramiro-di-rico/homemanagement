﻿using HomeManagement.API.Data;
using HomeManagement.API.Extensions;
using HomeManagement.API.Filters;
using HomeManagement.API.Infraestructure;
using HomeManagement.API.HostedServices;
using HomeManagement.API.Services;
using HomeManagement.FilesStore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;
using HomeManagement.Api.Core.Throttle;
using HomeManagement.Api.Core.HealthChecks;
using HomeManagement.API.Queue;

namespace HomeManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceCollection Services { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Services = services;

            services.AddLocalization(options => options.ResourcesPath = "Resource");

            var postgresConnection = Configuration.GetSection("ConnectionStrings").GetValue<string>("Postgres");
            services.AddDbContextPool<WebAppDbContext>(options =>
                options.UseNpgsql(postgresConnection));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(jwtBearerOptions =>
           {
               jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidateActor = false,
                   ValidateAudience = false,
                   ValidateLifetime = false,
                   ValidateIssuer = false,
                   ValidateIssuerSigningKey = false,
                   ValidIssuer = Configuration["Issuer"],
                   ValidAudience = Configuration["Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SigningKey"]))
               };
           });

            services.AddLogging();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRepositories();
            services.AddMiddleware();
            services.AddMappers();
            services.AddExportableComponents();
            services.AddCustomServices();
            services.AddQueryes();
            services.AddThrottlingService();

            services.AddScoped<ICurrencyService, CurrencyService>();

            services.AddScoped<IStorageClient, FilesStore.DropboxFileStore.RestClient>();

            services.AddHostedService<NotificationGeneratorHostedService>();
            services.AddHostedService<CurrencyUpdaterHostedService>();
            services.AddHostedService<BackupHostedService>();
            services.AddHostedService<EmailBackupHostedService>();
            services.AddHostedService<RegistrationServiceQueue>();
            services.AddHostedService<UserDeletionServiceQueue>();

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ThrottleFilter));
                options.Filters.Add(new ExceptionFilter());
            })
            .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName);

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Idnetity API", Version = "v1" });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                var securityScheme = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        new List<string>()
                    }
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder =>
                    corsBuilder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
                x.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddHealthChecks()
                .AddCheck<MemoryHealthCheck>("memory")
                .AddCheck("ping", new PingHealthCheck("www.google.com", 100));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddProvider(new DatabaseLoggerProvider(app.ApplicationServices));

            loggerFactory.AddFile("logs/logfile-{Date}.txt");

            app.EnsureDatabaseCreated(true);

            app.UseStaticFiles();

            app.UseRequestLocalization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Idnetity API V1");
            });

            app.UseRouting();

            app.UseCors("SiteCorsPolicy");

            app.UseAuthentication();

            app.UseEndpoints(x =>
            {
                x.MapDefaultControllerRoute();
            });
        }
    }
}
