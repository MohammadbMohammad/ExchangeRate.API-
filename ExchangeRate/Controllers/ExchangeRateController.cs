using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExchangeRate.Services;

namespace ExchangeRate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ExchangeRateService _exchangeRateService;

        public ExchangeRateController(ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency(string from, string to)
        {
            try
            {
                // Retrieve the conversion rate based on "from" and "to" inputs
                decimal conversionRate = await _exchangeRateService.GetConversionRate(from, to);

                // Create a response object with the conversion rate
                var response = new
                {
                    FromCurrency = from,
                    ToCurrency = to,
                    ConversionRate = conversionRate
                };

                return Ok(response);
            }
            catch (ExchangeRateServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("calculate")]
        public async Task<IActionResult> CalculateExchange(string from, string to, decimal amount)
        {
            try
            {
                // Retrieve the conversion rate based on "from" and "to" inputs
                decimal conversionRate = await _exchangeRateService.GetConversionRate(from, to);

                // Calculate the converted amount
                decimal convertedAmount = amount * conversionRate;

                // Create a response object with the converted amount
                var response = new
                {
                    FromCurrency = from,
                    ToCurrency = to,
                    Amount = amount,
                    ConvertedAmount = convertedAmount
                };

                return Ok(response);
            }
            catch (ExchangeRateServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
