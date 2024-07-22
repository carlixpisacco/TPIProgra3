using AutoMapper;
using ClothingStore.Application.IRepositories;
using ClothingStore.Application.IServices;
using ClothingStore.Application.Dtos;
using ClothingStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public UserService(IUserRepository repository, IMapper mapper, IProductRepository productRepository)
        {
            _userRepository = repository;
            _mapper = mapper;
            _productRepository = productRepository;
        }


        public ICollection<UserDTO> GetAllUsers(string role)
        {
            var users = _userRepository.GetAllUsers(role);
            return _mapper.Map<ICollection<UserDTO>>(users);
        }

        public UserWithProductsDTO? GetUserById(int id, string role)
        {
            var user = _userRepository.GetUserById(id, role);

            if (user == null) return null;

            // Mapear el usuario a UserWithProductsDTO
            var userDto = _mapper.Map<UserWithProductsDTO>(user);

            // Filtrar productos según el rol
            if (role == "client" && user is Client client)
            {
                userDto.UserProducts = client.PurchasedProducts
                    .Where(p => p.IsActive)
                    .Select(p => _mapper.Map<ProductDTO>(p)) // Mapear cada producto a ProductDTO
                    .ToList();
            }
            else if (role == "seller" && user is Seller seller)
            {
                userDto.UserProducts = seller.PublishedProducts
                    .Where(p => p.IsActive)
                    .Select(p => _mapper.Map<ProductDTO>(p)) // Mapear cada producto a ProductDTO
                    .ToList();
            }

            return userDto;
        }

        public UserDTO? AddUser(AddUserDTO addUserDto, string role)
        {
            
            var existingUser = _userRepository.GetUserByEmail(addUserDto.Email);
            if (existingUser != null)
            {
                return null; 
            }

            User newUser;

            if (role == "client")
            {
                newUser = new Client(); // Crear instancia de Client
            }
            else 
            {
                newUser = new Seller(); // Crear instancia de Seller
            }
         
            // Mapear propiedades del DTO al usuario creado
            _mapper.Map(addUserDto, newUser);
            newUser.Role = role;

            // Agregar el usuario al repositorio
            _userRepository.AddUser(newUser);

            // Mapear el usuario agregado de vuelta a DTO para retornar al controller
            return _mapper.Map<UserDTO>(newUser);
        }

        public UpdateUserResult UpdateUser(UpdateUserDTO updateUserDto, string role, int authenticatedUserId)
        {
            // Obtener el usuario existente desde el repositorio usando el rol
            var existingUser = _userRepository.GetUserById(updateUserDto.Id, role);

            if (existingUser == null)
            {
                return UpdateUserResult.NotFound; // Usuario no encontrado
            }

            // Verificar si el ID del usuario en el DTO coincide con el ID del usuario autenticado
            if (authenticatedUserId != updateUserDto.Id)
            {
                return UpdateUserResult.Unauthorized; // No tiene permiso para modificar este usuario
            }

            //actualizo manualmente los campos del usuario existente si el DTO contiene valores válidos, si no los tiene mantiene los de la base de datos para que no se mande "string"
            if (updateUserDto.Email != "user@example.com")
            {
                existingUser.Email = updateUserDto.Email;
              
            }
            if (updateUserDto.Password != "string")
            {
                existingUser.Password = updateUserDto.Password;
               
            }
            if (updateUserDto.UserName != "string")
            {
                existingUser.UserName = updateUserDto.UserName;
               
            }
            if (updateUserDto.Name != "string")
            {
                existingUser.Name = updateUserDto.Name;
                
            }
            if (updateUserDto.LastName != "string")
            {
                existingUser.LastName = updateUserDto.LastName;
               
            }

            // Actualizar el usuario en el repositorio
            _userRepository.UpdateUser(existingUser);

            return UpdateUserResult.Success;
        }

        public enum DeleteUserResult
        {
            Success,      // Eliminación exitosa
            NotFound,     // Usuario no encontrado
            Unauthorized  // No autorizado para eliminar el usuario
        }
        public DeleteUserResult DeleteUser(int id, string role, int authenticatedUserId)
        {
            // Obtener el usuario existente desde el repositorio usando el rol
            var user = _userRepository.GetUserById(id, role);

            if (user == null)
            {
                return DeleteUserResult.NotFound; // Usuario no encontrado
            }

            //verifico si el ID del usuario en el repositorio coincide con el ID del usuario autenticado
            if (authenticatedUserId != user.Id)
            {
                return DeleteUserResult.Unauthorized; // No tiene permiso para eliminar este usuario
            }

          
            user.IsActive = false; //elimino el usuario con baja lógica.
            _userRepository.UpdateUser(user); 

            if (role == "seller") //si es un vendedor, también eliminar los productos del mismo (baja lógica)
            {
                var products = _productRepository.GetProductsBySellerId(user.Id);
                foreach (var product in products)
                {
                    if (product.IsActive) // Solo desactivar productos activos
                    {
                        product.IsActive = false;
                        _productRepository.UpdateProduct(product);
                    }
                }
            }

            return DeleteUserResult.Success;
        }
    }
}



 






