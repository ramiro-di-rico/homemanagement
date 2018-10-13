﻿using HomeManagement.API.Data;
using HomeManagement.API.Data.Repositories;
using HomeManagement.API.Exportation;
using HomeManagement.API.Throttle;
using HomeManagement.Contracts;
using HomeManagement.Core.Cryptography;
using HomeManagement.Data;
using HomeManagement.Mapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HomeManagement.API.Extensions
{
    public static class StartUpExtensions
    {
        public static void AddMiddleware(this IServiceCollection services)
        {
            services.AddScoped<IThrottleCore, ThrottleCore>();

            services.AddScoped<ICryptography, AesCryptographyService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IPlatformContext, WebAppLayerContext>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<IChargeRepository, Data.Repositories.ChargeRepository>();

            services.AddScoped<ICategoryRepository, API.Data.Repositories.CategoryRepository>();

            services.AddScoped<IUserCategoryRepository, UserCategoryRepository>();

            services.AddScoped<IReminderRepository, ReminderRepository>();

            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddScoped<ITokenRepository, TokenRepository>();

            services.AddScoped<IWebClientRepository, WebClientRepository>();

            services.AddScoped<IDataLogRepository, DataLogRepository>();

            //with the throttle filter with persisted repo, the requests take around 100ms to respond
            //with memory values, it takes 30ms
            //services.AddScoped<IWebClientRepository, MemoryWebClientRepository>();            
        }

        public static void AddMappers(this IServiceCollection services)
        {
            services.AddScoped<ICategoryMapper, CategoryMapper>();

            services.AddScoped<IAccountMapper, AccountMapper>();

            services.AddScoped<IUserMapper, UserMapper>();

            services.AddScoped<IChargeMapper, ChargeMapper>();

            services.AddScoped<IReminderMapper, ReminderMapper>();

            services.AddScoped<INotificationMapper, NotificationMapper>();
        }

        public static void AddExportableComponents(this IServiceCollection services)
        {
            services.AddScoped<IExportableCategory, ExportableCategory>();

            services.AddScoped<IExportableCharge, ExportableCharge>();
        }

        public static CorsPolicy BuildCorsPolicy(this CorsOptions corsOptions)
        {
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();

            return corsBuilder.Build();
        }
    }
}