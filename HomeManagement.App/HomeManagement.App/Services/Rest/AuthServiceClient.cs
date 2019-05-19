﻿using Autofac;
using HomeManagement.App.Common;
using HomeManagement.Core.Caching;
using HomeManagement.Models;
using System;
using System.Threading.Tasks;

namespace HomeManagement.App.Services.Rest
{
    public class AuthServiceClient : IAuthServiceClient
    {
        public UserModel User { get; private set; }

        public async Task<UserModel> Login(UserModel user)
        {
            try
            {
                var result = await RestClientFactory
                    .CreateClient()
                    .PostAsync(Constants.Endpoints.Auth.LOGIN, user.SerializeToJson())
                    .ReadContent<UserModel>();

                //App._container.Resolve<ICachingService>().Store("header", result.Token);

                User = result;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Logout(UserModel user)
        {
            await RestClientFactory
                    .CreateAuthenticatedClient()
                    .PostAsync(Constants.Endpoints.Auth.LOGOUT, user.SerializeToJson())
                    .ReadContent<UserModel>();

            //App._container.Resolve<ICachingService>().Remove("header");
        }

        public async Task Logout()
        {
            await Logout(User);
        }
    }

    public interface IAuthServiceClient
    {
        Task<UserModel> Login(UserModel user);

        UserModel User { get; }

        Task Logout(UserModel user);
        Task Logout();
    }
}
