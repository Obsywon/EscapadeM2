﻿using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Escapade.Api.Repositories.Interfaces;
using Escapade.Api.Exceptions;
using Escapade.Api.Models;
using Escapade.Api.Services.Interfaces;
using Escapade.Api.Repositories;

namespace Escapade.Api.Services
{
    public class UserService : Service<User>, IUserService
    {
        public UserService(IRepositoryUser repository) : base(repository) { }

        public async Task<bool> IsEmailAlreadyExist(string email)
        {
            try
            {
                // Vérifiez si l'email existe déjà dans votre système
                var user = await _repository.GetByConditionAsync(x => x.Email == email);

                if (user != null)
                {
                    // L'email existe déjà, lancez une exception
                    throw new UserEmailTakenException(email);
                }

                // L'email n'existe pas encore
                return false;
            }
            catch (UserEmailTakenException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                User user = (await _repository.GetByConditionAsync(x => x.Email == email)).FirstOrDefault();
                if(user != null)
                    return user;
                throw new UserEmailNotFoundException(email);
            }
            catch (UserEmailNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> EncryptPasswordAsync(string password)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Convertir le mot de passe en un tableau de bytes
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                    // Utiliser MemoryStream pour envelopper le tableau de bytes
                    using (MemoryStream stream = new MemoryStream(passwordBytes))
                    {
                        // Calculer le hash de manière asynchrone
                        byte[] hashedBytes = await sha256.ComputeHashAsync(stream);

                        // Convertir le tableau de bytes en une chaîne hexadécimale
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < hashedBytes.Length; i++)
                        {
                            builder.Append(hashedBytes[i].ToString("x2"));
                        }

                        return builder.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void IsPasswordSecure(string password)
        {
            try
            {
                // Vérifier si le mot de passe a au moins 8 caractères
                if (password.Length < 8)
                {
                    throw new PasswordInvalidFormatException(password);
                }

                // Vérifier la présence de lettres minuscules, majuscules et chiffres dans le mot de passe
                string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";
                Regex regex = new Regex(passwordPattern);

                // Vérifier si le mot de passe correspond au format attendu
                if (!regex.IsMatch(password))
                {
                    throw new PasswordInvalidFormatException(password);
                }
            }
            catch (PasswordInvalidFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public void IsEmailFormatValid(string email)
        {
            try
            {
                // Utiliser une expression régulière pour valider le format de l'e-mail
                string emailPattern = @"^[a-z0-9._-]+@[a-z0-9.-]+\.[a-z]{2,4}$";
                Regex regex = new Regex(emailPattern, RegexOptions.IgnoreCase); 

                // Vérifier si l'e-mail correspond au format attendu
                if (!regex.IsMatch(email))
                {
                    throw new EmailInvalidFormatException(email);
                }
            }
            catch (EmailInvalidFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public void IsNameAndLastNameValid(string name, string lastName)
        {
            try
            {
                // Vérifier si le champ a au moins 3 caractères alphabétiques
                string nameAndLastNamePattern = @"^[a-zA-Z]{3,}$";
                Regex regex = new Regex(nameAndLastNamePattern);

                // Vérifier si le prénom correspond au format attendu
                if (!regex.IsMatch(name))
                    throw new NameInvalidFormatException(name);

                // Vérifier si le prénom correspond au format attendu
                if (!regex.IsMatch(lastName))
                    throw new LastnameInvalidFormatException(lastName);
            }
            catch (NameInvalidFormatException)
            {
                throw;
            }
            catch (LastnameInvalidFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void IsBirthDateValid(DateTime birthDate)
        {
            try
            {
                // Vérifier si la date de naissance est au format "DD-MM-YYYY"
                string datePattern = @"^\d{2}-\d{2}-\d{4}$";
                Regex regex = new Regex(datePattern);

                // Vérifier si la date de naissance correspond au format attendu
                if (!regex.IsMatch(birthDate.ToString("dd-MM-yyyy")))
                {
                    throw new BirthdateInvalidFormatException(birthDate.ToString());
                }
            }
            catch (BirthdateInvalidFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ICollection<Place>> GetAllFavoritePlacesAsync(string userId)
        {
            try
            {
                return await (_repository as UserRepository).GetFavoritePlacesByIUserdsAsync(userId);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<ICollection<Post>> GetRandomPostAsync()
        {
            try
            {
                // Récupérez tous les utilisateurs
                var allUsers = await (_repository as UserRepository).GetAllAsync();

                // Vérifiez s'il y a des utilisateurs disponibles
                if (allUsers == null || !allUsers.Any())
                {
                    // Gérez le cas où aucun utilisateur n'est disponible
                    return null;
                }

                // Fusionnez tous les posts de tous les utilisateurs
                var allPosts = allUsers.SelectMany(u => u.Posts).ToList();

                // Vérifiez s'il y a des posts disponibles
                if (allPosts == null || !allPosts.Any())
                {
                    // Gérez le cas où aucun post n'est disponible
                    return null;
                }

                // Générez une liste d'index aléatoires uniques
                var randomIndexes = Enumerable.Range(0, allPosts.Count).OrderBy(x => Guid.NewGuid()).Take(10).ToList();

                // Créez une liste pour stocker les posts aléatoires
                var randomPosts = new List<Post>();

                // Récupérez les posts correspondants aux index aléatoires
                foreach (var index in randomIndexes)
                {
                    randomPosts.Add(allPosts[index]);
                }

                return randomPosts;
            }
            catch (Exception ex)
            {
                // Vous pouvez logger l'exception ici
                throw;
            }
        }

        public async Task<ICollection<Post>> GetAllPostAsync()
        {
            try
            {
                // Récupérez tous les utilisateurs
                var users = await _repository.GetAllAsync();

                // Fusionnez tous les posts de tous les utilisateurs
                var allPosts = users.SelectMany(u => u.Posts).ToList();

                return allPosts;
            }
            catch (Exception ex)
            {
                // Vous pouvez logger l'exception ici
                throw;
            }
        }

        public async Task<ICollection<Post>> GetAllPostByUserAsync(string userId)
        {
            try
            {
                // Récupérez l'utilisateur correspondant à l'userId fourni
                var user = await _repository.GetByIdAsync(userId);

                // Vérifiez si l'utilisateur existe
                if (user == null)
                {
                    // Gérez le cas où l'utilisateur n'est pas trouvé
                    throw new Exception("L'utilisateur spécifié n'a pas été trouvé.");
                }

                // Retournez les posts de l'utilisateur
                return user.Posts;
            }
            catch (Exception ex)
            {
                // Vous pouvez logger l'exception ici
                throw;
            }
        }
    }
}
