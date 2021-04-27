using System;
using FluentValidation;
using Gateway.Domain;

namespace Gateway.DTOs.Validation
{
    public class CardDtoValidator : AbstractValidator<Card>
    {
        public CardDtoValidator()
        {
            RuleFor(c => c.Number)
                .Must(x => x.ToString().Length <= 16)
                .Must(x => x.ToString().Length >= 12);

            RuleFor(c => c.ExpiryMonth)
                .Must(x => x >= 0)
                .Must(x => x <= 12);

            RuleFor(c => c.ExpiryYear)
                .Must(x => x >= DateTime.Now.Year);

            RuleFor(c => c.Cvv)
                .Must(x => x.ToString().Length == 3);
        }
    }
}