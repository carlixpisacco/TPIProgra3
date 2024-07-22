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
     public class ProductRepository : IProductRepository
     {
         private readonly ClothingStoreDbContext _context;

         public ProductRepository(ClothingStoreDbContext context)
         {
            _context = context;
         }

        public ICollection <Product> GetAllAvailableAndActive()
        {
            return _context.Products
                .Include(p => p.Seller)  // Incluir los datos del vendedor
                .Where(p => p.IsActive && p.IsAvailable)
                .ToList();
        }

        public ICollection<Product> GetProductsBySellerId(int sellerId)
        {
            return _context.Products
                .Include(p => p.Client)
                .Where(p => p.SellerId == sellerId && p.IsActive)
                .ToList();
        }

        public ICollection<Product> GetPurchasedProductsByClientId(int clientId)
        {
            return _context.Products
                .Include(p => p.Seller)
                .Where(p => p.ClientId == clientId && p.IsActive)
                .ToList();
        }

        public Product? GetProductById(int productId)
        {
            return _context.Products
                .Include(p => p.Seller) // Incluir vendedor
                .Include(p => p.Client) // Incluir cliente (si es necesario)
                .FirstOrDefault(p => p.Id == productId);
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }
     }
}

