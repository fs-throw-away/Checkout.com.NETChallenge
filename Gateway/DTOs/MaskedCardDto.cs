using System;
using Gateway.Domain;
using Microsoft.VisualBasic;

namespace Gateway.DTOs
{
    public class MaskedCardDto
    {
        public string Number { get; set; }
        
        public int ExpiryMonth { get; set; }
        
        public int ExpiryYear { get; set; }
        
        public static MaskedCardDto fromCard(Card card)
        {
            var cardNumber = card.Number.ToString();
            var cardNumberLenght = cardNumber.Length;
            var maskedNumber = new String('*', cardNumberLenght - 4) 
                + cardNumber.Substring(cardNumberLenght - 4);
            
            return new MaskedCardDto()
            {
                Number = maskedNumber,
                ExpiryMonth = card.ExpiryMonth,
                ExpiryYear = card.ExpiryYear,
            };
        }
    }
}