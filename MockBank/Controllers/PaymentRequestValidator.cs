using FluentValidation;
using Gateway.Domain;
using Gateway.DTOs.Validation;

namespace MockBank.Controllers
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidator()
        {
            RuleFor(r => r.Card)
                .SetValidator(new CardDtoValidator());

            RuleFor(r => r.Amount)
                .Must(x => x > 0);

            RuleFor(r => r.Currency)
                .IsInEnum();
        }
    }
}