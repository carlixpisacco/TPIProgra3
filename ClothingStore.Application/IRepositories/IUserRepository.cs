using ClothingStore.Application.Dtos;
using ClothingStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.IRepositories
{
    public interface IUserRepository
    {
        ICollection<User> GetAllUsers(string role);
        User? GetUserById(int id, string role);
        User GetUserByIdSimple(int userId);
        void AddUser(User newUser);
        User? GetUserByEmail(string email);
        void UpdateUser(User user);
        void DeleteUser(User user);
        User? ValidateUser(AuthenticationRequestBody authenticationRequestBody);

    }
}
