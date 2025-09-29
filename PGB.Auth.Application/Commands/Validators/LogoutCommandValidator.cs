using FluentValidation;

namespace PGB.Auth.Application.Commands.Validators
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        #region Constructors
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token không được để trống");
        } 
        #endregion
    }
}