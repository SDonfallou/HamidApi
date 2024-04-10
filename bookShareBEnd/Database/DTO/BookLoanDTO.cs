namespace bookShareBEnd.Database.DTO
{
    public class BookLoanDTO
    {
        public Guid LoanId { get; set; }

        public Guid BookLoanId { get; set; }
        public Guid LoanerBookId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset LoanDate { get; set; }
    }
}
