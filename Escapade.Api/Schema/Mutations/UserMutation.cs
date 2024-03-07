﻿using Escapade.Api.Exceptions;
using Escapade.Api.Services.Interfaces;
using EscapadeApi.Services.Interfaces;
using Firebase.Auth.Requests;
using FirebaseAdmin.Auth;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Escapade.Api.Schema.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class UserMutation
    {
        #region HotChocolate

        private readonly IConfiguration _configuration;

        public UserMutation(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [Error(typeof(BirthDateInvalidFormatException))]
        [Error(typeof(EmailInvalidFormatException))]
        [Error(typeof(NameOrLastNameInvalidFormatException))]
        [Error(typeof(PasswordInvalidException))]
        public async Task<User> RegisterUserAsync(IUserService userService, string name, string lastname, string email, string password, DateTime birthDate, CancellationToken cancellationToken)
        {

            #region Verification Input

            userService.IsEmailFormatValid(email);
            userService.IsBirthDateValid(birthDate);
            userService.IsNameAndLastNameValid(name, lastname);
            userService.IsPasswordSecure(password);

            #endregion


            // Créer un nouvel utilisateur dans Firebase
            var firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                DisplayName = $"{name} {lastname}",
                Email = email,
                Password = password,
                EmailVerified = true,
                Disabled = false,
            }, cancellationToken);


            // Récupérer l'ID Firebase de l'utilisateur nouvellement créé
            string uidString = firebaseUser.Uid;


            // Récupérer le token Firebase associé à l'utilisateur
            //string firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uidString);


            // Créer un nouvel utilisateur
            User newUser = new User
            {
                Id = uidString,
                Name = name,
                LastName = lastname,
                Email = email,
                Password = password,
                BirthDate = birthDate,
                //Token = firebaseToken,
            };

            // Enregistrer l'utilisateur dans CosmoDb
            var user = await userService.CreateAsync(newUser);

            return user;       
        }

        [AllowAnonymous]
        [Error(typeof(BadRequestException))]
        public async Task<User> LoginUserAsync(IUserService userService, string email, string psw, CancellationToken cancellation)
        {
            // Récupérer l'utilisateur depuis votre service (par exemple, depuis CosmosDB) en utilisant l'email
            User user = await userService.GetUserByEmailAsync(email);


            using (var httpClient = new HttpClient())
            {
                string firebaseUri = _configuration["Firebase:Uri"];
                string firebaseApiKey = _configuration["Firebase:ApiKey"];

                string apiUrl = $"{firebaseUri}{firebaseApiKey}";

                var request = new
                {
                    email,
                    password=psw,
                    returnSecureToken = true
                };

                var response = await httpClient.PostAsJsonAsync(apiUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Désérialiser la réponse JSON
                    var responseObject = JsonConvert.DeserializeObject<VerifyPasswordResponse>(responseContent);

                    // Accéder à la propriété idToken
                    string idToken = responseObject.IdToken;

                    user.Token = idToken;
                    var userUpdated = await userService.UpdateAsync(user);

                    return userUpdated;
                }
                throw new BadRequestException(await response.Content.ReadAsStringAsync());
            }

        }

        [Authorize]
        public async Task<User> AddNewFavoritePlaceToThisUserAsync(string userId, IUserService userService, IPlaceService placeService, IHttpContextAccessor httpContextAccessor, string placeId, CancellationToken cancellationToken)
        {

            User currentUser = null;
            Place currentPlace;

            if (await userService.IsFoundAsync(userId))
                currentUser = await userService.GetByIdAsync(userId);

            if(await placeService.IsFoundAsync(placeId))
                currentPlace = await placeService.GetByIdAsync(userId);

            FavoritePlace favoritePlace = new FavoritePlace();
            favoritePlace.PlaceId = placeId;

            currentUser.FavoritePlaces.Add(favoritePlace);

            return await userService.UpdateAsync(currentUser);
        }

        [Authorize]
        public async Task<User> UpdateThisUserAsync(string userId, IUserService userService, IHttpContextAccessor httpContextAccessor, string name, string lastname, DateTime birthDate, string gender , CancellationToken cancellationToken)
        {

            User currentUser = null;

            if (await userService.IsFoundAsync(userId))
                currentUser = await userService.GetByIdAsync(userId);

            currentUser.Name = name;
            currentUser.LastName = lastname;
            currentUser.BirthDate = birthDate;
            currentUser.Gender = gender;

            return await userService.UpdateAsync(currentUser);
        }

        #endregion
    }
}
