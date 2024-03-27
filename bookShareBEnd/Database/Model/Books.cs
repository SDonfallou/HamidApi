namespace bookShareBEnd.Database.Model
{
    public class Books
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int YearPublished { get; set; }

        public string Cover { get; set; }

        public String Description { get; set; }

        public Guid UserId { get; set; }

    }
}
