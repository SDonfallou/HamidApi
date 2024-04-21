﻿namespace bookShareBEnd.Database.DTO
{
    public class BookDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int YearPublished { get; set; }

        public string Cover { get; set; } = string.Empty;

        public string? BookCity { get; set; }


        public string Description { get; set; }
        public string Category { get; set; }

        public int Likes { get; set; }

        public Guid UserId { get; set; }
    }

    public class BookDTOSimple
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int YearPublished { get; set;}
        public string Description { get; set; }

        public string Category { get; set; }

        public int Likes { get; set; }

    }
}
