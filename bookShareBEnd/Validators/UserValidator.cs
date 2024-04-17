using bookShareBEnd.Database.DTO;
using FluentValidation;

namespace bookShareBEnd.Validators
{
    public class UserValidator : AbstractValidator<UserAuthDTO>
    {
        public UserValidator() 
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");
            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 Characters long");
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");
           

        }

    }
}
