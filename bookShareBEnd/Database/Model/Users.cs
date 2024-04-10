namespace bookShareBEnd.Database.Model
{

    public class Users
    {

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }


        public string Email { get; set; }

        public string Password { get; set; }
        public Guid RoleId { get; set; }


        public ICollection<BookLoan> BookLoans { get; set; }

        public ICollection<Likes> Likes { get; set; }

    }
}
