using bookShareBEnd.Database.DTO;
using bookShareBEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bookShareBEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private RolesService _rolesService;
        public RolesController(RolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpPost("Add-role")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddRole([FromBody] RolesDTO role)
        {
            _rolesService.AddRole(role);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetAllRole()
        {
            var allRoles = _rolesService.GetAllRoles();
            return Ok(allRoles);
        }
        [HttpGet("Get-roles-id/{id}")]
        public IActionResult GetRole(Guid id)
        {
            var role = _rolesService.GetRoleById(id);
            return Ok(role);
        }

        [HttpPut("update-role-by-id/{id}")]
        public IActionResult UpdateRole(Guid id, [FromBody] RolesDTO role)
        {
            var existingRole = _rolesService.GetRoleById(id);
            if (existingRole == null)
            {
                return NotFound($"Role with ID {id} not found.");
            }

            existingRole.Label = role.Label; // Update properties of the existing role

            _rolesService.UpdateRole(existingRole); // Update the role in the database

            return Ok(existingRole); // Return the updated role
        }



        [HttpDelete("delete-role-by-id{id}")]
        public IActionResult DeleteRole(Guid id)
        {
            _rolesService.DeleteRoleById(id);
            return Ok();
        }

        private UserDTO GetCurrent()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity is not null)
            {
                var userClaims = identity.Claims;
                return new UserDTO
                {
                    Name = GetValueByClaimType(userClaims, ClaimTypes.Name),
                    Email = GetValueByClaimType(userClaims, ClaimTypes.Email),
                    RoleId = (Guid)GetRoleId(userClaims)
                };
            }
            return null;
        }

        private string GetValueByClaimType(IEnumerable<Claim> claims, string claimType)
        {
            return claims.FirstOrDefault(o => o.Type == claimType)?.Value;
        }

        private Guid? GetRoleId(IEnumerable<Claim> claims)
        {
            var roleClaim = claims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value;

            if (Guid.TryParse(roleClaim, out var roleId))
            {
                return roleId;
            }

            return null;
        }

    }
}
