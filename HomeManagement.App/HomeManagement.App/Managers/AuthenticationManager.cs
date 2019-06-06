﻿using Autofac;
using HomeManagement.App.Data;
using HomeManagement.App.Data.Entities;
using HomeManagement.App.Services.Rest;
using HomeManagement.Contracts;
using HomeManagement.Core.Caching;
using HomeManagement.Models;
using System;
using System.Threading.Tasks;

namespace HomeManagement.App.Managers
{
    public interface IAuthenticationManager
    {
        Task<User> AuthenticateAsync(string username, string password);

        User GetAuthenticatedUser();

        Task Logout();
    }

    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly ICryptography crypto = App._container.Resolve<ICryptography>();
        private readonly IAuthServiceClient authServiceClient = App._container.Resolve<IAuthServiceClient>();
        private readonly ICachingService cachingService = App._container.Resolve<ICachingService>();
        private readonly GenericRepository<User> userRepository = new GenericRepository<User>();
        private readonly GenericRepository<AppSettings> appSettingsRepository = new GenericRepository<AppSettings>();

        private User user;

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var encryptedPassword = crypto.Encrypt(password);

            user = userRepository.FirstOrDefault(x => x.Email.Equals(username));

            if (user != null && user.Password.Equals(encryptedPassword) && (DateTime.Now - user.LastApiCall).Hours < 1)
            {
                CacheUserAndToken(user);
                return user;
            }

            var userModel = await authServiceClient.Login(new UserModel { Email = username, Password = encryptedPassword });

            if (user == null)
            {
                user = new User
                {
                    Id = userModel.Id,
                    Email = userModel.Email,
                    Password = encryptedPassword,
                    ChangeStamp = DateTime.Now,
                    LastApiCall = DateTime.Now,
                    Token = userModel.Token
                };
            }
            else
            {
                user.Token = userModel.Token;
                user.LastApiCall = DateTime.Now;
            }


            SaveOrUpdateUser(user);

            CreateSettingsIfNotExits();

            CacheUserAndToken(user);

            return user;
        }

        public User GetAuthenticatedUser() => cachingService.Get<User>("user");

        public async Task Logout()
        {
            //await authServiceClient.Logout();
            if (cachingService.Exists("header"))
            {
                cachingService.Remove("header");
            }

            if (cachingService.Exists("user"))
            {
                cachingService.Remove("user");
            }
        }

        private void CacheUserAndToken(User user)
        {
            SaveHttpHeader(user.Token);
            SaveUser(user);
        }

        private void SaveUser(User user)
        {
            if (!cachingService.Exists("user"))
            {
                cachingService.Store("user", user);
            }
            else
            {
                cachingService.Remove("user");
                cachingService.Store("user", user);
            }
        }

        private void SaveHttpHeader(string token)
        {
            if (!cachingService.Exists("header"))
            {
                cachingService.Store("header", token);
            }
            else
            {
                cachingService.Remove("header");
                cachingService.Store("header", token);
            }
        }

        private void SaveOrUpdateUser(User user)
        {
            if(userRepository.Any(x => x.Email.Equals(user.Email)))
            {
                userRepository.Update(user);
            }
            else
            {
                userRepository.Add(user);
            }            

            userRepository.Commit();
        }

        private void CreateSettingsIfNotExits()
        {
            var cloudSyncName = AppSettings.GetCloudSyncSetting();
            var settings = appSettingsRepository.FirstOrDefault(x => x.Name.Equals(cloudSyncName.Name));

            if (settings == null)
            {
                appSettingsRepository.Add(cloudSyncName);
                appSettingsRepository.Commit();
            }
        }
    }
}