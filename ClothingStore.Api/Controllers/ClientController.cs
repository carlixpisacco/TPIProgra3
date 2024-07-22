
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
    [Authorize(Roles = "client")] // Aplicar autorización a nivel de clase
    public class ClientController : ControllerBase
    {
        private readonly IUserService _userService;

        public ClientController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllClients")] //consulta por todos los clientes activos en la base de datos. 
        public ActionResult<ICollection<UserDTO>> GetAllClients()
        {
            var clients = _userService.GetAllUsers("client");

            if (!clients.Any())
            {
                return NotFound("No se encontraron clientes en la base de datos");
            }

            return Ok(clients);
        }

        [HttpGet("GetClientById/{id}")] //consulta por el cliente mediante id, solo puede consultar por sus datos. Además trae los productos que compró y se encuentran activos (no fueron eliminados por el vendedor que los publico)
        public ActionResult<UserWithProductsDTO?> GetClientById([FromRoute] int id)
        {
            // Obtener el ID del usuario autenticado desde las reclamaciones
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("ID del cliente no válido o no proporcionado.");
            }

            // Verifica si el cliente existe en la base de datos
            var client = _userService.GetUserById(id, "client");

            if (client == null)
            {
                return NotFound("No se encontró el cliente en la base de datos");
            }

            // Verifica si el ID solicitado coincide con el ID del usuario autenticado
            if (userId != id)
            {
                return Unauthorized("No tiene permiso para acceder a la información de este usuario");
            }

            return Ok(client);
        }

        [HttpPost("AddNewClient")] //endpoint para agregar nuevo cliente (con rol "client") es decir "registrarse" como cliente. No es necesario iniciar sesión.
        [AllowAnonymous] // Esta acción permite acceso anónimo
        public ActionResult<UserDTO> AddClient([FromBody] AddUserDTO addUserDto)
        {
            var newClientDto = _userService.AddUser(addUserDto, "client");

            if (newClientDto == null)
            {
                return BadRequest("El email ya está en uso. Pruebe con otro email");
            }
         
            // construir la URL del nuevo recurso
            var locationUrl = Url.Action(
            nameof(GetClientById), // Nombre del método de acción que manejará la solicitud
            "Seller", // Nombre del controlador
            new { productId = newClientDto.Id }, // Parámetros de la ruta
            Request.Scheme // Esquema (http o https)
            );

            return Ok(newClientDto);
        }

        [HttpPut("UpdateClient")] //permite modificar los datos del cliente que inició sesión, mediante su id, solo puede modificar sus datos. 
        public IActionResult UpdateClient([FromBody] UpdateUserDTO updateUserDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("ID del cliente no válido o no proporcionado.");
            }

            var result = _userService.UpdateUser(updateUserDto, "client", userId);

            switch (result)
            {
                case UpdateUserResult.NotFound:
                    return NotFound("No existe cliente con el id indicado. Pruebe con otro id");
                case UpdateUserResult.Unauthorized:
                    return Unauthorized("No tiene permiso para modificar la información de este usuario");
                default:
                    return Ok("El cliente fue modificado con éxito");
            }
        }

        [HttpDelete("DeleteClient/{id}")] //permite eliminar (mediante baja lógica) al cliente que inició sesión, solo puede eliminar su propio usuario. 
        public IActionResult DeleteClient([FromRoute] int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("ID del cliente no válido o no proporcionado.");
            }

            var result = _userService.DeleteUser(id, "client", userId);

            switch (result)
            {
                case DeleteUserResult.NotFound:
                    return NotFound("No se encontró el cliente en la base de datos. Pruebe con otro Id");
                case DeleteUserResult.Unauthorized:
                    return Unauthorized("No tiene permiso para eliminar este usuario");
                default:
                    return Ok("El cliente fue eliminado de la base de datos");
            }
        }
    }
}