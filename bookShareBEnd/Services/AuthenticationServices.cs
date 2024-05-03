using BCrypt.Net;
using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace bookShareBEnd.Services
{
    public class AuthenticationServices
    {
        private AppDbContext _context;
        private  IConfiguration _configuration;
        public AuthenticationServices( AppDbContext context, IConfiguration configuration)
        {
            _context = context;   
            _configuration = configuration;
        }

        public UserDTO Authentication(UserAuthDTOSimple userLogin) 
        {
            var cryptedPassword = BCrypt.Net.BCrypt.HashPassword(userLogin.Password);
            var user = _context.users.FirstOrDefault(u => u.Email == userLogin.Email);
           if (user is  null)
           {
               throw new Exception("User doesn't exist");
           }
            if (! BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                throw new Exception("incorrect password ");
            }
            UserDTO authentificedUser = new UserDTO
            {
                UserId= user.UserId,
                RoleId = user.RoleId,
                Name = user.Name,
                Email = user.Email
            };
            return authentificedUser;
           
        }

        public string Generate(UserDTO user)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      

            var claims = new[]
                          {
                                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Assuming UserId is a Guid or an integer
                                new Claim(ClaimTypes.Name, user.Name),
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.Role, user.RoleId.ToString()), // Assuming RoleId is a Guid
                            };
            var token = new JwtSecurityToken(
                              issuer: _configuration["Jwt:Issuer"],
                              audience: _configuration["Jwt:Audience"],
                              claims: claims,
                              expires: DateTime.Now.AddMinutes(50), // Token expiration time
                              signingCredentials: credentials
                         );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
