using bookShareBEnd.Database.DTO;
using bookShareBEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace bookShareBEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UsersServices usersService;
        private AuthenticationServices _authenticationServices;
        private IValidator<UserAuthDTO> _validator;
        public UsersController(UsersServices usersServices, AuthenticationServices authenticationServices,IValidator<UserAuthDTO> validator)
        {
            usersService = usersServices;
            _authenticationServices = authenticationServices;
            _validator = validator;
        }

        [HttpGet("Get-All-Users")]
        public IActionResult GetAllUsers()
        {
            var allUsers = usersService.GetAllUsers();
            return Ok(allUsers);
        }

        [HttpPost("add-user")] // custom api posso anche  cancellarlo
        public async Task<IActionResult> AddUserAsync([FromBody]UserAuthDTO user)
        {
            // Validate the user using FluentValidation

            var validationResult = await _validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                // If validation fails, return the validation errors
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }

            // If the user is valid, add it using the service
            await  usersService.AddUserAssigned(user);
            return Ok("User added successfully.");
        }

        [HttpPost("updateOrCreate-User")]
        public IActionResult CreateOrUpdateUser(Guid? userId, [FromBody]UserAuthDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Check if user data is provided in the request body
            if (user == null)
            {
                return BadRequest("User data is missing.");
            }

            // If userId is null, create a new user
            if (!userId.HasValue)
            {
                usersService.AddUserAssigned(user);
                return Ok("User created successfully.");
            }
            else
            {
                // Validate if the provided userId is valid
                if (userId == null || userId == Guid.Empty)
                {
                    return BadRequest("Invalid user ID provided.");
                }

                // Update the existing user
                var updatedUser = usersService.UpdateUserById(userId.Value, user);
                if (updatedUser == null)
                {
                    return NotFound("User not found.");
                }

                return Ok("User updated successfully.");
            }
        }


        [HttpGet("Get-user-id/{userId}")]
        public IActionResult GetUsers(Guid userId)
        {
            var user = usersService.GetUserById(userId);
            return Ok(user);
        }

        [HttpPut("update-user-by-id/{userId}")] 
        public async Task<IActionResult> UpdateUserByIdAsync(Guid userId, [FromBody] UserAuthDTO user)
        {

            var validationResult = await _validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }
            var updateUser = usersService.UpdateUserById(userId, user);

            return Ok(updateUser);
        }

        [HttpDelete("Delete-user-by-Id/{Id}")]
        public IActionResult DeleteById(Guid Id)
        {
            usersService.DeleteUserById(Id);
            return Ok();
        }

    }
}
