using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace bookShareBEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private  readonly AppDbContext _context;
        private UsersServices _usersServices;
        private AuthenticationServices _authenticationServices;
        private readonly IValidator<UserAuthDTO> _validator;


        public AuthenticationController(IConfiguration configuration,
                                         AppDbContext context,
                                         AuthenticationServices authenticationServices,
                                         UsersServices usersServices,
                                         IValidator<UserAuthDTO> validator)
        {
            _configuration = configuration;
            _context = context;
            _authenticationServices = authenticationServices;
            _usersServices = usersServices;
            _validator = validator;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserAuthDTO loginDTO)
        {
            var user = _authenticationServices.Authentication(loginDTO);
            if (user is not  null) 
            {
              var token = _authenticationServices.Generate(user);
                if (token is not null)
                {
                    throw new Exception("Not Valid Token ");
                }
              return Ok(token);
            }
            return NotFound("User not Found");
        }


        [AllowAnonymous]
        [HttpPost("Registration")]
        public IActionResult Registration([FromBody] UserAuthDTO userDTO)
        {
            var validationResult = _validator.Validate(userDTO);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }
          var userRegistred =  _usersServices.AddUserAssigned(userDTO);
            return Ok();
        }


    }
}
