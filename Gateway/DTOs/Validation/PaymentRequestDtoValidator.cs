using FluentValidation;

namespace Gateway.DTOs.Validation
{
    public class PaymentRequestDtoValidator : AbstractValidator<PaymentRequestDtoV1>
    {
        public PaymentRequestDtoValidator()
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