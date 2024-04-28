using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;
using bookShareBEnd.Database;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace bookShareBEnd.Services
{
    public class UsersServices
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly TimeSpan _debounceDelay = TimeSpan.FromSeconds(1);
        private CancellationTokenSource _debounceCancellationTokenSource;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsersServices(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsUserAuthenticated()
        {
            // Check if the current user is authenticated
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public UserDTO UpdateUserById(Guid userId, [FromBody] UserAuthDTO user)
        {
           
            var userNew = _context.users.FirstOrDefault(x => x.UserId == userId);
            if (userNew == null)
            {
                throw new Exception("User is not validate");
            }
            if (user is not null)
            {
                _mapper.Map(user, userNew);
                if (userNew != null && !string.IsNullOrEmpty(user.Password))
                {
                    userNew.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }
                
                _context.SaveChanges();
                return _mapper.Map<UserDTO>(userNew);
            }
            return null; //TODO the handler Exception
        }
        public List<UserDTO> GetAllUsers()
        {
           
             var users = _context.users.ToList();
            var usersDTO = _mapper.Map<List<UserDTO>>(users);
            return usersDTO;
        }
        public Users GetUserById(Guid userId)
        {
            var user = _context.users.FirstOrDefault(x => x.UserId == userId);
            return user;
        }

        public async Task AddUser([FromBody] UserAuthDTO user)
        {
            var _user = _mapper.Map<UserAuthDTO, Users>(user);

            // Retrieve the list of roles from the database
            var roles =  _context.roles.ToList();
            
            var email = user.Email.Trim().ToLower();

            if(_context.users.Any(x => x.Email.Equals(email)))
            {
                throw new Exception("Email is already in use");
            }

            // Validate the provided role ID
            var isValidRoleId = roles.Any(role => role.Id == _user.RoleId);
            if (!isValidRoleId || roles == null || !roles.Any())
            {
                throw new Exception("Role ID is not valid");
            }

            // Hash the password
            _user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Add the user to the context
            _context.users.Add(_user);

            // Save changes
            await _context.SaveChangesAsync();
        }




        //public async task<list<likesdto>> getalllikesbyuser(guid userid)
        //{

        //}

        public async Task AddUserAssigned([FromBody] UserAuthDTO user)
        {
            var _user = _mapper.Map<UserAuthDTO, Users>(user);
            var email = user.Email.Trim().ToLower();

            // Check if the email is already in use
            if (_context.users.Any(x => x.Email.Equals(email)))
            {
                throw new Exception("Email is already in use");
            }

            // Hash the password
            _user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Query the role based on the label provided in UserDTO
            var role = _context.roles.FirstOrDefault(r => r.Label == "user");

            // Check if the role exists
            if (role == null)
            {
                throw new Exception("Role does not exist");
            }

            // Assign the role ID to the user
            _user.RoleId = role.Id;

            // Add the user to the context and save changes
            _context.users.Add(_user);
            _context.SaveChanges();
        }



        public async Task  DeleteUserById(Guid id)
        {
                    var user = _context.users.FirstOrDefault(c => c.UserId == id);

                    if (user is  null)
                    {
                       throw new Exception("user doesn't exist");
                    }
                    
                // Find all books associated with the user and set UserId to null
                var relatedBooks = _context.books.Where(b => b.UserId == id).ToList();
                foreach (var book in relatedBooks)
                {
                    book.UserId = Guid.Empty; // Set UserId to Guid.Empty
                }

                // Save changes to the database to update the foreign key values
                _context.SaveChanges();

                // Remove the user from the Users table
                _context.users.Remove(user);
                _context.SaveChanges();
            }

        public async Task<UserSearchDTO> SearchUser(string searchUser)
        {

            try
            {
                // Cancel the previous debounce task if it exists
                _debounceCancellationTokenSource?.Cancel();

                // Create a new cancellation token source for the debounce task
                _debounceCancellationTokenSource = new CancellationTokenSource();

                // Wait for the debounce delay
                await Task.Delay(_debounceDelay, _debounceCancellationTokenSource.Token);

                // Actual search logic here
                var user = await SearchUserAsync(searchUser);

                if (user == null) throw new Exception("User doesn't exist");
                    

                // Convert the Book entity to userSearchDTO
                var userdto = new UserSearchDTO
                {
                    Name = user.Name,
                   
                };

                return userdto;
            }
            catch(Exception ex)
            {
                throw new Exception("Error search " + ex);
            }

           
        }
           



       private async Task<UserDTO> SearchUserAsync(string searchUser)
        {
            var searchIndb = _context.users.FirstOrDefault(b => b.Name.Contains(searchUser) || b.Email.Contains(searchUser));
            var search = _mapper.Map<UserDTO>(searchIndb);
            return search;

        }


    }
}
