using ClothingStore.Application.Dtos;
using ClothingStore.Application.IRepositories;
using ClothingStore.Domain.Entities;
using ClothingStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ClothingStoreDbContext _context;

        public UserRepository(ClothingStoreDbContext context)
        {
            _context = context;
        }

        //si bien "user" es una clase abstracta,
        //puedo instanciarlas gracias a que en el db context lo configure para que al recibir el "role",
        //en realidad se esté instanciando alguna de las clases derivadas, es decir se esta instanciando un objeto "client" o "seller" 
        //ver en el db context la linea que permite esto. 
        public ICollection<User> GetAllUsers(string role) 
        {
            return _context.Users.Where(u => u.Role == role && u.IsActive).ToList();
        }

        public User GetUserByIdSimple(int userId)
        {
            return _context.Users.SingleOrDefault(u => u.Id == userId && u.IsActive);
        }

        public User? GetUserById(int id, string role)
        {
      
              var user = _context.Users
                  .Where(u => u.Id == id && u.Role == role && u.IsActive)
                  .FirstOrDefault();

              if (user != null)
              {
                 // Cargar productos asociados dependiendo del rol
                 if (role == "client" && user is Client client)
                 {
                      _context.Entry(client)
                          .Collection(c => c.PurchasedProducts)
                          .Query()
                          .Where(p => p.IsActive)
                          .Load();
                 }
                 else if (role == "seller" && user is Seller seller)
                 {
                      _context.Entry(seller)
                          .Collection(s => s.PublishedProducts)
                          .Query()
                          .Where(p => p.IsActive)
                          .Load();
                  }
              }

              return user;  
        }

        public void AddUser(User newUser)
        {
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.IsActive);
        }

        public void UpdateUser(User updateUser)
        {
            _context.Users.Update(updateUser);
            _context.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User? ValidateUser(AuthenticationRequestBody authenticationRequestBody)
        {
            return _context.Users.FirstOrDefault(c => c.UserName == authenticationRequestBody.UserName && c.Password == authenticationRequestBody.Password && c.IsActive); //si no encuentra el usuario, devuelve null
        }

    }
}
