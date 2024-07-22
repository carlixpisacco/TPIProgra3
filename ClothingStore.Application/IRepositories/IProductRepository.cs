using ClothingStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.IRepositories
{
    public interface IProductRepository
    {
        ICollection<Product> GetAllAvailableAndActive();
        ICollection<Product> GetProductsBySellerId(int sellerId);
        ICollection<Product> GetPurchasedProductsByClientId(int clientId);
        Product? GetProductById(int productId);
        void AddProduct(Product product);
        void UpdateProduct(Product product);



    }
}
