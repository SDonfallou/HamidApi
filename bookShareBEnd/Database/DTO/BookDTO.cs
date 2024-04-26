namespace bookShareBEnd.Database.DTO
{
    public class BookDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int YearPublished { get; set; }

        public string Cover { get; set; } = string.Empty;

       // public string? BookCity { get; set; }
        public string ShortDescription { get; set; }
        public string Category { get; set; }
        public string FullDescription { get; set; }
        public int Likes { get; set; }
        public int Pages { get; set; }

        public string UserName {  get; set; }
        public string City { get; set; }
        public Guid UserId { get; set; }
    }

    public class BookDTOSimple
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int YearPublished { get; set;}
        public string ShortDescription { get; set; }

        public string Category { get; set; }

        public int Likes { get; set; }

    }
}
