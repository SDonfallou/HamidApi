using System.Globalization;

namespace bookShareBEnd.Database.DTO
{
    public class UserDTO
    {
       // public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }    
        public string Password { get; set; }
        public Guid RoleId { get; set; }

    }
}
