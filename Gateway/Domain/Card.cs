namespace Gateway.Domain
{
    public class Card
    {
        public ulong Number { get; set; }
        
        public int ExpiryMonth { get; set; }
        
        public int ExpiryYear { get; set; }
        
        public int Cvv { get; set; }
    }
}