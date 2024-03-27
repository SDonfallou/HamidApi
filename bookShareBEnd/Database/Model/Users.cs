namespace bookShareBEnd.Database.Model
{

    public class Users
    {
        // [Key]
        public Guid UserId { get; set; }

        //[Required(ErrorMessage = "Label is required")]
        public string Name { get; set; }

        public string UserName { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        // [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        //  [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        public Guid RoleId { get; set; }
    }
}
