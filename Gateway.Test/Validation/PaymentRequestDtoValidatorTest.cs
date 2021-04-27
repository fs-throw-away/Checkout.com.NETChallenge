using FluentValidation.TestHelper;
using Gateway.Domain;
using Gateway.DTOs.Validation;
using Gateway.Test.utils;
using Xunit;

namespace Gateway.Test.Validation
{
    public class PaymentRequestDtoValidatorTest
    {
        private readonly PaymentRequestDtoValidator _paymentRequestDtoValidator = new();

        [Fact]
        public void should_have_error_if_amount_is_negative()
        {
            var paymentRequestDto = Factory.ValidPaymentRequestDto();
            paymentRequestDto.Amount = (decimal) -0.001;
            
            _paymentRequestDtoValidator.TestValidate(paymentRequestDto)
                .ShouldHaveValidationErrorFor(p => p.Amount);
        }
        
        [Fact]
        public void should_have_error_if_card_is_invalid()
        {
            var paymentRequestDto = Factory.ValidPaymentRequestDto();
            paymentRequestDto.Card.Cvv = 12345;
            
            _paymentRequestDtoValidator.TestValidate(paymentRequestDto)
                .ShouldHaveValidationErrorFor(p => p.Card.Cvv);
        }
    }
}