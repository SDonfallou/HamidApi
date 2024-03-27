using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;
using bookShareBEnd.Database;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace bookShareBEnd.Services
{
    public class UsersServices
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;
        public UsersServices(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public UserDTO UpdateUserById(Guid userId, [FromBody] UserDTO user)
        {
            var userNew = _context.users.FirstOrDefault(x => x.UserId == userId);
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

        public void AddUser([FromBody] UserDTO user)
        {
         
            var _user = _mapper.Map<UserDTO,Users>(user);
            _user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.users.Add(_user);
            _context.SaveChanges();
        }


        public void DeleteUserById(Guid id)
        {
            var user = _context.users.FirstOrDefault(c => c.UserId == id);

            if (user != null)
            {
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
        }



    }
}
