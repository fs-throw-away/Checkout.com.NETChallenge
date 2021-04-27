using FluentValidation.TestHelper;
using Gateway.Test.utils;
using MockBank.Controllers;
using Xunit;

namespace MockBank.Test
{
    public class PaymentRequestValidatorTest
    {
        private readonly PaymentRequestValidator _paymentRequestValidator = new();

        [Fact]
        public void should_have_error_if_amount_is_negative()
        {
            var paymentRequest = Factory.ValidPaymentRequest();
            paymentRequest.Amount = (decimal) -0.001;
            
            _paymentRequestValidator.TestValidate(paymentRequest)
                .ShouldHaveValidationErrorFor(p => p.Amount);
        }
        
        [Fact]
        public void should_have_error_if_card_is_invalid()
        {
            var paymentRequest = Factory.ValidPaymentRequest();
            paymentRequest.Card.Cvv = 12345;
            
            _paymentRequestValidator.TestValidate(paymentRequest)
                .ShouldHaveValidationErrorFor(p => p.Card.Cvv);
        }
    }
}