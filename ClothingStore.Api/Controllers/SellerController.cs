
using ClothingStore.Application.IServices;
using ClothingStore.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClothingStore.Application.Services;
using ClothingStore.Domain.Entities;
using static ClothingStore.Application.Services.UserService;

namespace ClothingStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "seller")] // Aplicar autorización a nivel de clase
    public class SellerController : ControllerBase
    {
        private readonly IUserService _userService;

        public SellerController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllSellers")] //consulta por todos los vendedores activos en la base de datos. 
        public ActionResult<ICollection<UserDTO>> GetAllSellers()
        {
            var sellers = _userService.GetAllUsers("seller");

            if (!sellers.Any())
            {
                return NotFound("No se encontraron vendedores en la base de datos");
            }

            return Ok(sellers);
        }

        [HttpGet("GetSellerById/{id}")]  //consulta por el vendedor mediante id, solo puede consultar por sus datos. Además trae los productos que publicó y se encuentran activos (no fueron eliminados por èl)
        public ActionResult<UserWithProductsDTO?> GetSellerById([FromRoute] int id)
        {
            // Obtener el ID del usuario autenticado desde las reclamaciones
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("ID del vendedor no válido o no proporcionado.");
            }


            // Verifica si el vendedor existe en la base de datos
            var seller = _userService.GetUserById(id, "seller");

            if (seller == null)
            {
                return NotFound("No se encontró el vendedor en la base de datos");
            }

            // Verifica si el ID solicitado coincide con el ID del usuario autenticado
            if (userId != id)
            {
                return Unauthorized("No tiene permiso para acceder a la información de este usuario");
            }

            return Ok(seller);
        }

        [HttpPost("AddNewSeller")] //endpoint para agregar nuevo vendedor (con rol "seller") es decir "registrarse" como vendedor. No es necesario iniciar sesión.
        [AllowAnonymous] // Esta acción permite acceso anónimo
        public ActionResult<UserDTO> AddSeller([FromBody] AddUserDTO addUserDto)
        {
            var newSellerDto = _userService.AddUser(addUserDto, "seller");

            if (newSellerDto == null)
            {
                return BadRequest("El email ya está en uso. Pruebe con otro email");
            }

            // construir la URL del nuevo recurso
            var locationUrl = Url.Action(
            nameof(GetSellerById), // Nombre del método de acción que manejará la solicitud
            "Seller", // Nombre del controlador
            new { productId = newSellerDto.Id }, // Parámetros de la ruta
            Request.Scheme // Esquema (http o https)
            );

            return Created(locationUrl, newSellerDto);
        }
         
        [HttpPut("UpdateSeller")] //permite modificar los datos del vendedor que inicio sesión, mediante su id, solo puede modificar sus datos. 
        public IActionResult UpdateSeller([FromBody] UpdateUserDTO updateUserDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("ID del vendedor no válido o no proporcionado.");
            }

            var result = _userService.UpdateUser(updateUserDto, "seller", userId);

            switch (result)
            {
                case UpdateUserResult.NotFound:
                    return NotFound("No existe vendedor con el id indicado. Pruebe con otro id");
                case UpdateUserResult.Unauthorized:
                    return Unauthorized("No tiene permiso para modificar la información de este usuario");
                default:
                    return Ok("El vendedor fue modificado con éxito");
            }
        }

        [HttpDelete("DeleteSeller/{id}")] //permite eliminar (mediante baja lógica) al vendedor que inició sesión, solo puede eliminar su propio usuario. Al eliminarse se eliminan tambièn sus productos (mediante baja lógica) 
        public IActionResult DeleteSeller([FromRoute] int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("ID del vendedor no válido o no proporcionado.");
            }

            var result = _userService.DeleteUser(id, "seller", userId);

            switch (result)
            {
                case DeleteUserResult.NotFound:
                    return NotFound("No se encontró el vendedor en la base de datos. Pruebe con otro Id");
                case DeleteUserResult.Unauthorized:
                    return Unauthorized("No tiene permiso para eliminar este usuario");
                default:
                    return Ok("El vendedor fue eliminado de la base de datos junto con sus productos");
            }
        }
    }
}