namespace bookShareBEnd.Database.DTO
{
    public class LikesDTO
    {
        public Guid Id { get; set; }
        public Guid BookID { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset LikedAt { get; set; }
    }
}
