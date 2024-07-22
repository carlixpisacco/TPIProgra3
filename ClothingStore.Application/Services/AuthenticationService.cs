using ClothingStore.Application.Dtos;
using ClothingStore.Application.IRepositories;
using ClothingStore.Application.IServices;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public AuthenticationUserModel? ValidateCredentials(AuthenticationRequestBody authenticationRequestBody)
        {
            if (string.IsNullOrEmpty(authenticationRequestBody.UserName) || string.IsNullOrEmpty(authenticationRequestBody.Password))
                return null;

            var user = _userRepository.ValidateUser(authenticationRequestBody);

            if (user == null)
                return null;

            // Mapear la entidad User al modelo AuthenticatedUserModel
            var authenticatedUser = new AuthenticationUserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role,
                Email = user.Email
            };

            return authenticatedUser;
        }
    }
   }

