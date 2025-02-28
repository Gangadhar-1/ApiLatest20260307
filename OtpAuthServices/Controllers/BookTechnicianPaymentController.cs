using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;
using System.Net;
using System.Text;

namespace OtpAuthServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookTechnicianPaymentController : ControllerBase
    {
        private readonly ICosmosDbService<BookTechnicianPayment> _cosmosDbService;

        public BookTechnicianPaymentController(ICosmosDbService<BookTechnicianPayment> cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpPost("CreateBookTechnicianPayment")]
        public async Task<IActionResult> CreateTicket([FromBody] BookTechnicianPayment BookTechnicianPayment)
        {
            if (BookTechnicianPayment == null)
            {
                return BadRequest("BookTechnicianPayment data cannot be null.");
            }

            try
            {
                string dynmaicTechrrotp = GenerateRandomOtp();





                BookTechnicianPayment.id = Guid.NewGuid().ToString();
                BookTechnicianPayment.PaymentId = Guid.NewGuid().ToString();
                //payment.PaymentDataTime = DateTime.UtcNow;
                BookTechnicianPayment.TechnicianConfirmationCode = dynmaicTechrrotp;

                await _cosmosDbService.AddItemAsync(BookTechnicianPayment);
                return Ok(new
                {
                    Message = "BookTechnicianPayment created successfully",
                    BookTechnicianPaymentId = BookTechnicianPayment.id,
                    TechnicianConfirmationCode = BookTechnicianPayment.TechnicianConfirmationCode
                });
            }
            catch (Exception ex)
            {
                // Log the exception here using a logging framework (e.g., Serilog, NLog, etc.)
                return StatusCode(500, new { Message = "An error occurred while creating the payment.", Details = ex.Message });
            }
        }

        private string GenerateRandomOtp()
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                int randomNumber = BitConverter.ToInt32(bytes, 0);
                return (Math.Abs(randomNumber % 900000) + 100000).ToString("D6");
            }
        }



        [HttpGet("GetBookTechnicianPaymentById")]
        public async Task<IActionResult> GetPaymentByPamentId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("BookTechnicianPayment ID cannot be null or empty.");
            }

            var ticket = await _cosmosDbService.GetItemAsync(id);
            if (ticket == null)
            {
                return NotFound($"BookTechnicianPayment with ID {id} not found.");
            }

            return Ok(ticket);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(string id, [FromBody] BookTechnicianPayment BookTechnicianPayment)
        {
            if (BookTechnicianPayment == null || BookTechnicianPayment.id != id)
            {
                return BadRequest("BookTechnicianPayment information is incorrect.");
            }

            var existingPayment = await _cosmosDbService.GetItemAsync(id);
            if (existingPayment == null)
            {
                existingPayment.PaymentId = BookTechnicianPayment.PaymentId;



            }

            await _cosmosDbService.UpdateItemAsync(BookTechnicianPayment);
            return Ok($"BookTechnicianPayment Data Updated Successfully. At with respectiveId {id}.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(string id)
        {
            var existingpayment = await _cosmosDbService.GetItemAsync(id);
            if (existingpayment == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync(id);
            return Ok("Successfully  deleted  BookTechnicianPayment  Item. ");
        }
    }
}


