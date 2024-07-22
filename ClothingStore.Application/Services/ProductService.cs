using AutoMapper;
using ClothingStore.Application.Dtos;
using ClothingStore.Application.IRepositories;
using ClothingStore.Application.IServices;
using ClothingStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.Services
{   public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public ProductService(IProductRepository productRepository,  IMapper mapper, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public ICollection<ProductWithSellerDTO> GetAllAvailableAndActiveProducts()
        {
            // Obtén los productos del repositorio
            var products = _productRepository.GetAllAvailableAndActive();

            // Devuelve los productos mapeados a ProductwithsellerDto en una sola línea
            return _mapper.Map<ICollection<ProductWithSellerDTO>>(products);
        }

        public ICollection<ProductWithClientDTO> GetProductsBySellerId(int sellerId)
        {
            // Obtén los productos del repositorio
            var products = _productRepository.GetProductsBySellerId(sellerId);

            // Mapea los productos a ProductWithClientDto
            return _mapper.Map<ICollection<ProductWithClientDTO>>(products);
        }

        public ICollection<ProductWithSellerDTO> GetPurchasedProductsByClientId(int clientId)
        {
            // Obtén los productos del repositorio
            var products = _productRepository.GetPurchasedProductsByClientId(clientId);

            // Mapea los productos a ProductwithsellerDto
            return _mapper.Map<ICollection<ProductWithSellerDTO>>(products);
        }

        public ProductWithSellerDTO GetProductForClient(int productId)
        {
            var product = _productRepository.GetProductById(productId);

            if (product == null || !product.IsAvailable || !product.IsActive)
            {
                return null; 
            }

            return _mapper.Map<ProductWithSellerDTO>(product);
        }

        public ProductWithClientDTO GetProductForSeller(int productId, int sellerId)
        {
            // Obtener el producto por ID
            var product = _productRepository.GetProductById(productId);

            // Verificar si el producto existe y está activo
            if (product == null || !product.IsActive)
            {
                return null; // Producto no válido
            }

            // Verificar si el producto pertenece al vendedor que realiza la solicitud
            if (product.SellerId != sellerId)
            {
                return null; // El vendedor no tiene permiso para ver este producto
            }

            // Mapear el producto a ProductWithClientDTO
            return _mapper.Map<ProductWithClientDTO>(product);
        }

        public ProductDTO AddProduct(AddProductDTO productDto, int sellerId)
        {
            //verifico si el vendededor está activo. 
            var user = _userRepository.GetUserByIdSimple(sellerId);
            if (user == null)
            {
                return null;
            }
            // Mapeo de AddProductDTO a Product
            var product = _mapper.Map<Product>(productDto);

            // Establecer IsAvailable e IsActive en true por defecto
            product.IsAvailable = true;
            product.IsActive = true;

            // Establecer el SellerId
            product.SellerId = sellerId;

            // Guardar el producto en el repositorio
            _productRepository.AddProduct(product);

            // Mapeo de Product a ProductDTO para devolverlo
            return _mapper.Map<ProductDTO>(product);
        }

        public bool PurchaseProduct(int productId, int clientId)
        {
            var product = _productRepository.GetProductById(productId);

            if (product == null || !product.IsAvailable || !product.IsActive)
            {
                return false; // Producto no encontrado o no disponible para compra
            }

            product.IsAvailable = false;
            product.ClientId = clientId;

            _productRepository.UpdateProduct(product);

            return true; // Compra exitosa
        }

        public UpdateProductResult UpdateProduct(int productId, UpdateProductDTO updateProductDto, int sellerId)
        {
            // Obtener el producto existente desde el repositorio
            var existingProduct = _productRepository.GetProductById(productId);

            if (existingProduct == null)  
            {
                return UpdateProductResult.NotFound; // Producto no encontrado
            }

            if (!existingProduct.IsActive || !existingProduct.IsAvailable)
            {
                return UpdateProductResult.NotFound2; // Producto no encontrado o no disponible para actualización
            }

            // Verificar si el producto pertenece al vendedor autenticado
            if (existingProduct.SellerId != sellerId)
            {
                return UpdateProductResult.Unauthorized; // No tiene permiso para modificar este producto
            }

            // Actualizar los campos del producto if (updateUserDto.Password != "string")
          
            if (updateProductDto.Description != "string")
            {
                existingProduct.Description = updateProductDto.Description;
            }
            if (updateProductDto.Price != 0)
            {
                existingProduct.Price = updateProductDto.Price;
            }

            // Actualizar el producto en el repositorio
            _productRepository.UpdateProduct(existingProduct);

            return UpdateProductResult.Success;
        }

        public enum DeleteProductResult
        {
            Success,
            NotFound,
            Unauthorized,
            AlreadyInactive
        }

        public DeleteProductResult DeleteProduct(int productId, int sellerId)
        {
            var existingProduct = _productRepository.GetProductById(productId);

            if (existingProduct == null)
            {
                return DeleteProductResult.NotFound; // Producto no encontrado
            }

            if (existingProduct.SellerId != sellerId)
            {
                return DeleteProductResult.Unauthorized; // No tiene permiso para desactivar este producto
            }

            if (!existingProduct.IsActive)
            {
                return DeleteProductResult.AlreadyInactive; // El producto ya está inactivo
            }

            existingProduct.IsActive = false;

            _productRepository.UpdateProduct(existingProduct);

            return DeleteProductResult.Success;
        }


    }
}
