using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gateway.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gateway.Services
{
    public class AcquiringBankService : IAcquiringBankService
    {
        private readonly ILogger<AcquiringBankService> _logger;
        private HttpClient Client { get; }

        public AcquiringBankService(ILogger<AcquiringBankService> logger, HttpClient client, IConfiguration configuration)
        {
            _logger = logger;
            client.BaseAddress = new Uri(configuration.GetValue<string>("AcquiringBankUrl"));
            Client = client;
        }
        
        public async Task<BankPaymentResponse> Process(PaymentRequest paymentRequest)
        {
            _logger.LogInformation($"Processing {nameof(paymentRequest)}");
            
            var httpResponse = await Client.PostAsync("/bankpayment", new StringContent(
                JsonSerializer.Serialize(paymentRequest),
                Encoding.UTF8,
                "application/json"));

            var body = await httpResponse.Content.ReadAsStringAsync();
            
            _logger.LogInformation(body);
            
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.Created:
                    var bankPaymentResponse = JsonSerializer.Deserialize<BankPaymentResponse>(body);
                    if (bankPaymentResponse == null)
                    {
                        var message = $"Error deserializing bank payment response body: {body}";
                        _logger.LogError(message);
                        throw new AcquiringBankServiceException(message);
                    }
                    
                    _logger.LogInformation($"Returning response status {bankPaymentResponse.BankPaymentStatus}");
                    return bankPaymentResponse;
                default:
                    var errorMessage = $"Received response code {httpResponse.StatusCode} from {Client.BaseAddress}: {body}";
                    _logger.LogError(errorMessage);
                    throw new AcquiringBankServiceException(errorMessage);
            }
        }
    }
}