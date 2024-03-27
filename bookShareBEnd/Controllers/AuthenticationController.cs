using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom.Compiler;

namespace bookShareBEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private  AppDbContext _context;
        private UsersServices _usersServices;
        private AuthenticationServices _authenticationServices;

        public AuthenticationController(IConfiguration configuration, AppDbContext context, AuthenticationServices authenticationServices,UsersServices usersServices)
        {
            _configuration = configuration;
            _context = context;
            _authenticationServices = authenticationServices;
            _usersServices = usersServices;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var user = _authenticationServices.Authentication(loginDTO);
            if (user is not  null) 
            {
              var token = _authenticationServices.Generate(user);
              return Ok(token);
            }
            return NotFound("User not Found");
        }

        [HttpPost("Registration")]
        public IActionResult Registraction([FromBody]UserDTO userDTO)
        {
           _usersServices.AddUser(userDTO);
            return Ok();
        }



    }
}
