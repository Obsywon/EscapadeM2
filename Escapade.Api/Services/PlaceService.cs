﻿using Escapade.Api.Exceptions;
using Escapade.Api.Models;
using Escapade.Api.Repositories;
using Escapade.Api.Repositories.Interfaces;
using Escapade.Api.Services.Interfaces;
using EscapadeApi.Repositories;
using EscapadeApi.Repositories.Interfaces;
using EscapadeApi.Services.Interfaces;

namespace Escapade.Api.Services
{
    public class PlaceService : Service<Place>, IPlaceService
    {
        public PlaceService(IRepositoryPlace repository) : base(repository) { }

        public async Task<ICollection<PlaceAddedByUser>> GetAllPlaceAddedByUser(string userId)
        {
            try
            {
                return (ICollection<PlaceAddedByUser>)await (_repository as PlaceAddedByUserRepository).GetByConditionAsync(place => place.UserId == userId);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
