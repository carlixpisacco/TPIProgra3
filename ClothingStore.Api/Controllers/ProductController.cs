using ClothingStore.Application.Dtos;
using ClothingStore.Application.IServices;
using ClothingStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static ClothingStore.Application.Services.ProductService;

namespace ClothingStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        //ENDPOINTS COMUNES.
        [HttpGet("GetProductById/{productId}")] //permite consultar por un producto mediante su Id. 
        //en caso de "cliente" puede consultar por cualquier producto que se encuentre activo y no haya sido vendido, muestra el nombre el precio y datos del vendedor que lo publicó.
        //en caso de "vendedor" puede consultar únicamente por los productos que él publicó, si el producto se vendió le informa esto y le muestra los datos del comprador, si no se vendió le figura "este producto no se vendió"
        [Authorize]
        public IActionResult GetProductById(int productId)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("ID no válido.");
            }

            if (userRole == "client")
            {
                var productDto = _productService.GetProductForClient(productId);

                if (productDto == null)
                {
                    return NotFound("Producto no encontrado en la base de datos.Pruebe otro id.");
                }

                return Ok(productDto);
            }
            else if (userRole == "seller")
            {
                var productDto = _productService.GetProductForSeller(productId, userId);

                if (productDto == null)
                {
                    return NotFound("Producto no encontrado en la base de datos. Pruebe otro id.");
                }

                return Ok(productDto);
            }

            return BadRequest("Rol de usuario no reconocido.");
        }

        //ENDOPOINTS DEL CLIENTE.

        [HttpGet("GetAvailableAndActiveProducts")] //trae todos los productos que no fueron eliminados ni comprados. 
        [Authorize(Roles = "client")]
        public ActionResult<ICollection<ProductWithSellerDTO>> GetAvailableAndActiveProducts()
        {
            // Obtén los productos disponibles y activos del servicio
            var products = _productService.GetAllAvailableAndActiveProducts();

            // Verifica si la colección de productos está vacía
            if (products == null || !products.Any())
            {
                // Retorna un código 404 si no se encontraron productos
                return NotFound("No se encontraron productos disponibles en la base de datos.");
            }

            // Retorna un código 200 OK con la lista de productos
            return Ok(products);
        }

        [HttpGet("GetPurchasedProducts")] //trae los productos que compró siempre que el vendeedor que los publicó no los haya eliminado. 
        [Authorize(Roles = "client")]
        public ActionResult<ICollection<ProductWithSellerDTO>> GetPurchasedProductsByClientId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("ID del cliente no válido.");
            }

            // Obtener productos comprados por el cliente
            var products = _productService.GetPurchasedProductsByClientId(userId);

            if (products == null || !products.Any())
            {
                return NotFound("No se encontraron productos comprados por el cliente.");
            }

            return Ok(products);
        }

        [HttpPost("PurchaseProduct/{productId}")] //permite "comprar" un producto poniendo como false un booleano (distinto al active de eliminado)
        [Authorize(Roles = "client")]
        public IActionResult PurchaseProduct(int productId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int clientId))
            {
                return BadRequest("ID del cliente no válido.");
            }

            var success = _productService.PurchaseProduct(productId, clientId);

            if (!success)
            {
                return BadRequest("No se pudo realizar la compra. El producto no está disponible o no existe.");
            }

            return Ok("Producto comprado exitosamente.");
        }

        //ENDPOINTS DEL VENDEDOR.

        [HttpGet("GetSellerProducts")]
        [Authorize(Roles = "seller")]
        public ActionResult<ICollection<ProductWithClientDTO>> GetProductsBySellerId() //trae todos los productos publicados por el vendedor, que no haya eliminado. Informa sobre si fueron vendidos o no, si fueron vendidos da informaciòn del cliente que lo compro. 
        {
            // Obtener el ID del vendedor desde los claims del usuario
            var sellerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Verificar si el ID del vendedor es válido
            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return BadRequest("Id del vendedor no válido.");  //el badrequest(código de estado HTTP 400) es el código de estado adecuado para indicar
                                                                  //que la solicitud del cliente no es válida debido a que el ID del vendedor
                                                                  //es incorrecto o está malformado
            }

            // Obtener los productos del vendedor desde el servicio
            var products = _productService.GetProductsBySellerId(sellerId);

            // Verificar si se encontraron productos
            if (products == null || !products.Any())
            {
                return NotFound("No se encontraron productos para el vendedor.");
            }

            // Retornar los productos con un código 200 OK
            return Ok(products);
        }

        [HttpPost("AddNewProduct")] //permite agregar nuevos productos, únicamente reservdo al rol "vendedor"
        [Authorize(Roles = "seller")] 
        public ActionResult<ProductDTO> AddProduct([FromBody] AddProductDTO productDto)
        {
            // Obtener el ID del vendedor desde el claim
            var sellerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return BadRequest("Id del vendedor no válido.");
            }

            // Llamar al servicio para crear el producto
            var newProductDto = _productService.AddProduct(productDto, sellerId);

            if (newProductDto == null)
            {
                return NotFound("El usuario no se encontró o no está activo.");
            }

            //armar ruta del nuevo producto. 
            var locationUrl = Url.Action(
            nameof(GetProductById), // Nombre del método de acción que manejará la solicitud
            "Product", // Nombre del controlador
            new { productId = newProductDto.Id }, // Parámetros de la ruta
            Request.Scheme // Esquema (http o https)
            );

            return Created(locationUrl, newProductDto);
        }
    

        [HttpPut("UpdateProduct")]
        [Authorize(Roles = "seller")] //permite al vendedor modificar los productos que publicó, mediante id de los mismos,  solo puede modificar los propios.
        public IActionResult UpdateProduct([FromBody] UpdateProductDTO updateProductDto)
        {
            var sellerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return BadRequest("ID del vendedor no válido.");
            }

            var result = _productService.UpdateProduct(updateProductDto.Id, updateProductDto, sellerId);

            switch (result)
            {
                case UpdateProductResult.NotFound:
                    return NotFound("No existe producto con el id ingresado. Pruebe con otro Id.");
                case UpdateProductResult.NotFound2:
                    return NotFound("El producto no puede ser modificado porque fue vendido o eliminado.");
                case UpdateProductResult.Unauthorized:
                    return Unauthorized("No tiene permiso para modificar este producto.");
                default:
                    return Ok("El producto fue modificado con éxito.");
            }
        }

        [HttpDelete("DeactivateProduct/{productId}")] //permite elimminar los productos que publicó mediante id, solo puede eliminar los suyos. (elimina mediante baja lógica)0
        [Authorize(Roles = "seller")]
        public IActionResult DeleteProduct([FromRoute] int productId)
        {
            var sellerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return BadRequest("ID del vendedor no válido.");
            }

            var result = _productService.DeleteProduct(productId, sellerId);

            switch (result)
            {
                case DeleteProductResult.NotFound:
                    return NotFound("No existe producto con el ID indicado.");
                case DeleteProductResult.Unauthorized:
                    return Unauthorized("No tiene permiso para desactivar este producto.");
                case DeleteProductResult.AlreadyInactive:
                    return BadRequest("El producto ya fue eliminado anteriormente.");
                default:
                    return Ok("El producto fue eliminado con éxito.");
            }
        }

    }
}
    

