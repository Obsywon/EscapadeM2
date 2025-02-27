﻿using Escapade.Api.Models;
using Escapade.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Escapade.Api.Repositories
{
    public class UserRepository : Repository<User>, IRepositoryUser
    {
        public UserRepository(CosmosContext dbContext) : base(dbContext) { }


        public async Task<ICollection<Place>> GetFavoritePlacesByIUserdsAsync(string userId)
        {
            var user = await GetByIdAsync(userId);

            //if (user?.FavoritePlaces == null || !user.FavoritePlaces.Any())
            //{
            //    return new List<Place>();
            //}

            var favoritePlaceIds = user.FavoritePlaces.Select(fp => fp.PlaceId).ToList();

            // Utiliser la méthode GetFavoritePlacesByIdsAsync pour récupérer les lieux favoris
            return await _dbContext.Set<Place>().Where(place => favoritePlaceIds.Contains(place.Id)).ToListAsync();
        }

    }
}
