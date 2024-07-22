using ClothingStore.Application.Dtos;
using ClothingStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClothingStore.Application.Services.ProductService;

namespace ClothingStore.Application.IServices
{
    public interface IProductService
    {
        ICollection<ProductWithSellerDTO> GetAllAvailableAndActiveProducts();
        ICollection<ProductWithClientDTO> GetProductsBySellerId(int sellerId);
        ICollection<ProductWithSellerDTO> GetPurchasedProductsByClientId(int clientId);
        ProductWithSellerDTO GetProductForClient(int productId);
        ProductWithClientDTO GetProductForSeller(int productId, int userId);
        ProductDTO? AddProduct(AddProductDTO productDto, int sellerId);
        bool PurchaseProduct(int productId, int clientId);
        UpdateProductResult UpdateProduct(int productId, UpdateProductDTO updateProductDto, int sellerId);
        DeleteProductResult DeleteProduct(int productId, int sellerId);
    }
}
