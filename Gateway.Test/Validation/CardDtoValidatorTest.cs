using FluentValidation.TestHelper;
using Gateway.DTOs.Validation;
using Gateway.Test.utils;
using Xunit;

namespace Gateway.Test.Validation
{
    public class CardDtoValidatorTest
    {
        private readonly CardDtoValidator _cardDtoValidator = new CardDtoValidator();

        [Fact]
        public void should_have_error_when_number_lenght_is_out_of_bounds()
        {
            var card = Factory.ValidCard();
            card.Number = 123;

            _cardDtoValidator.TestValidate(card)
                .ShouldHaveValidationErrorFor(c => c.Number);

            card.Number = 12356789012345678;
            
            _cardDtoValidator.TestValidate(card)
                .ShouldHaveValidationErrorFor(c => c.Number);
        }
        
        [Fact]
        public void should_have_error_when_expiry_month_is_invalid()
        {
            var card = Factory.ValidCard();
            card.ExpiryMonth = -1;

            _cardDtoValidator.TestValidate(card)
                .ShouldHaveValidationErrorFor(c => c.ExpiryMonth);

            card.ExpiryMonth = 13;
            
            _cardDtoValidator.TestValidate(card)
                .ShouldHaveValidationErrorFor(c => c.ExpiryMonth);
        }
        
        [Fact]
        public void should_have_error_when_expiry_year_is_past()
        {
            var card = Factory.ValidCard();
            card.ExpiryYear = 2020;

            _cardDtoValidator.TestValidate(card)
                .ShouldHaveValidationErrorFor(c => c.ExpiryYear);
        }
        
        [Fact]
        public void should_have_error_when_Cvv_is_not_three_characters()
        {
            var card = Factory.ValidCard();
            card.Cvv = 1234;

            _cardDtoValidator.TestValidate(card)
                .ShouldHaveValidationErrorFor(c => c.Cvv);

            card.Cvv = 4;
            
            _cardDtoValidator.TestValidate(card)
                .ShouldHaveValidationErrorFor(c => c.Cvv);
        }
    }
}