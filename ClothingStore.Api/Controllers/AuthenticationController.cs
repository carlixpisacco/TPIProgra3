using ClothingStore.Application.Dtos;
using ClothingStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ClothingStore.Api.Controllers
{
    [Route("api/[controller]")]  //el endpoint de esta controller va a ser el encargado de chequear la identidad del usuario y devolver un jwt
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _config;
        public AuthenticationController(IAuthenticationService authenticationService, IConfiguration config)
        {
            _authenticationService = authenticationService;
            _config = config; //Hacemos la inyección para poder usar el appsettings.json
        }

        [HttpPost] //se usa un post porque además de que necesitamos enviar las credenciales por el body (lo cual permite el post)
                   //tmb se utiliza porque es la creación de algo, no necesariamente en la base de datos, en este caso es la creación de una "sesión" o "acceso"
                   //si bien se devuelve algo y podríamos usar un get, no es lo único que hacemos y por eso lo más lógico y completo para usar es el post. 
        public ActionResult<string> Authenticate([FromBody] AuthenticationRequestBody authenticationRequestBody)
        {
            //Validar credenciales
            var user = _authenticationService.ValidateCredentials(authenticationRequestBody); //en el servicio instancio a user que tiene 

            if (user is null)
                return Unauthorized();

            //Creo el Token
            var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"])); //Traemos la SecretKey del Json.

            var credentials = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);

            //Los claims son datos en clave->valor que nos permite guardar data del usuario.
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.Id.ToString())); //"sub" es una key estándar que significa unique user identifier, es decir, si mandamos el id del usuario por convención lo hacemos con la key "sub".
            claimsForToken.Add(new Claim("username", user.UserName));
            claimsForToken.Add(new Claim(ClaimTypes.Role, user.Role));

            var jwtSecurityToken = new JwtSecurityToken( // Acá es donde se crea el token con toda la data que le pasamos antes.
              _config["Authentication:Issuer"],
              _config["Authentication:Audience"],
              claimsForToken,
              DateTime.UtcNow,
              DateTime.UtcNow.AddHours(1),
              credentials);

            var tokenToReturn = new JwtSecurityTokenHandler() ////Pasamos el token a string
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

    }
}

