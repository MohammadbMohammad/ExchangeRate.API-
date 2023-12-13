using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeRate.Services
{
    public class ExchangeRateService
    {
        private const string BaseUrl = "https://v6.exchangerate-api.com/v6/46daf2616f64708e5279905f/latest/";
        private readonly HttpClient _httpClient;

        public ExchangeRateService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<decimal> ConvertCurrency(string from, string to, decimal amount)
        {
            string apiUrl = $"{BaseUrl}/{from}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                ExchangeRateApiResponse apiResponse = JsonConvert.DeserializeObject<ExchangeRateApiResponse>(json);

                if (!apiResponse.Success)
                {
                    throw new ExchangeRateServiceException(apiResponse.Error);
                }

                if (!apiResponse.ConversionRates.ContainsKey(to))
                {
                    throw new ExchangeRateServiceException($"Conversion rate for currency '{to}' not found.");
                }

                decimal rate = apiResponse.ConversionRates[to];
                decimal convertedAmount = amount * rate;

                return convertedAmount;
            }
            catch (Exception ex)
            {
                throw new ExchangeRateServiceException("An error occurred while converting the currency.", ex);
            }
        }

        public async Task<decimal> GetConversionRate(string from, string to)
        {
            // Set the amount to 1 since we are retrieving the conversion rate only
            decimal amount = 1;

            try
            {
                // Call the ConvertCurrency method passing the from, to, and amount
                decimal conversionRate = await ConvertCurrency(from, to, amount);

                return conversionRate;
            }
            catch (ExchangeRateServiceException ex)
            {
                // Handle the exception or rethrow if needed
                throw new Exception("An error occurred while retrieving the conversion rate.", ex);
            }
        }
    }

    public class ExchangeRateServiceException : Exception
    {
        public ExchangeRateServiceException(string message) : base(message)
        {
        }

        public ExchangeRateServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ExchangeRateApiResponse
    {
        [JsonProperty("result")]
        public string? Result { get; set; }

        [JsonProperty("conversion_rates")]
        public Dictionary<string, decimal>? ConversionRates { get; set; }

        [JsonProperty("error-type")]
        public string? ErrorType { get; set; }

        [JsonProperty("error-info")]
        public string? ErrorInfo { get; set; }

        public bool Success => string.IsNullOrEmpty(ErrorType) && string.IsNullOrEmpty(ErrorInfo);
        public string Error => $"{ErrorType}: {ErrorInfo}";
    }
}
