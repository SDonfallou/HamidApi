using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;
using FluentValidation;
using System.Data;

namespace bookShareBEnd.Validators
{
    public class BookValidator : AbstractValidator<BookDTO>
    {
        public BookValidator()
        {
            RuleFor(books => books.Title)
              .NotEmpty();

            RuleFor(books => books.Cover)
                .NotEmpty()
                .When(books => !string.IsNullOrEmpty(books.Cover)); // Only validate Cover further if it's not empty

            RuleFor(books => books.Author)
                .NotEmpty();

            RuleFor(books => books.YearPublished)
                .NotEmpty()

                .GreaterThan(0) // Assuming YearPublished cannot be negative
                ;

            RuleFor(books => books.UserId)
                .NotEmpty();

            RuleFor(books => books.ShortDescription)
                .NotEmpty()
                .MaximumLength(200); // Assuming a maximum length for the description
            RuleFor(books => books.FullDescription)
                .NotEmpty()
                .MaximumLength(10000);
            RuleFor(books => books.Likes)
                .NotEmpty()

                .GreaterThan(0) // Assuming YearPublished cannot be negative
                ;
        }

    }
}
