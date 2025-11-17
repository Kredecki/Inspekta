using FluentValidation;
using Inspekta.Shared.DTOs;
using Microsoft.Extensions.Localization;

namespace Inspekta.Client.Validators;

internal sealed class SignInValidator : AbstractValidator<SignInDto>
{
    public SignInValidator(IStringLocalizer<SignInValidator> localizer)
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage(localizer["login_required"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["password_required"]);
    }
}
