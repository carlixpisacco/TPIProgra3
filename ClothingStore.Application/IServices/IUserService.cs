using ClothingStore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClothingStore.Application.Services.UserService;

namespace ClothingStore.Application.IServices
{
    public interface IUserService
    {
        public ICollection<UserDTO> GetAllUsers(string role);
        public UserWithProductsDTO? GetUserById(int id, string role);
        public UserDTO? AddUser(AddUserDTO addUserDto, string role);
        UpdateUserResult UpdateUser(UpdateUserDTO updateUserDto, string role, int authenticatedUserId);
        DeleteUserResult DeleteUser(int id, string role, int authenticatedUserId);
    }
}
