namespace bookShareBEnd.Database.Model
{
    public class Likes
    {
        public Guid Id { get; set; }
        public Guid BookID { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset likedAt { get; set; }
    }
}
