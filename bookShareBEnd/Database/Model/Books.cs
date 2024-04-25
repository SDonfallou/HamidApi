namespace bookShareBEnd.Database.Model
{
    public class Books
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int YearPublished { get; set; }

        public string Cover { get; set; }

        public String ShortDescription { get; set; }
        public String FullDescription { get; set; }
        public string Category { get; set; }

        //public string bookCity { get; set; }

        public int Likes { get; set; }
        public int Pages { get; set; }
        public Guid UserId { get; set; }

    }
}
