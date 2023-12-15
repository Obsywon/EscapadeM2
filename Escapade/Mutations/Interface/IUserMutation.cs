﻿using AzureFunctionEscapade.Queries.Interface;
using AzureFunctionEscapade.Queries;
using HotChocolate;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctionEscapade.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctionEscapade.Mutations.Interface
{
    public interface IUserMutation : IMutation<User>
    {
        public Task<User> CreateUserRestApi(IHttpClientFactory clientFactory, User newUser, CancellationToken cancellationToken);

        public Task<User> UpdateUserRestApi(IHttpClientFactory clientFactory, string userId, User updatedUser, CancellationToken cancellationToken);

        public Task DeleteUserRestApi(IHttpClientFactory clientFactory, string userId, CancellationToken cancellationToken);

    }
}
