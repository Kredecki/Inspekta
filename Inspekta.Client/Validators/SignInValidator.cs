using FluentValidation;
using Inspekta.Shared.DTOs;

namespace Inspekta.Client.Validators;

internal sealed class SignInValidator : AbstractValidator<SignInDto>
{
    public SignInValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required.")
            .MinimumLength(4).WithMessage("Login must be at least 4 characters long.")
            .MaximumLength(20).WithMessage("Login must not exceed 20 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
    }
}
