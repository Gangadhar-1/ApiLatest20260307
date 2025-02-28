
using Microsoft.AspNetCore.Mvc;
using OtpAuthServices.AzureService;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Twilio.Rest.Api.V2010.Account;
using OtpAuthServices.Models;

namespace OtpAuthServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookTechnicianAddressController : ControllerBase
    {
        private readonly ICosmosDbService<BookTechnicianAddress> _cosmosDbService;
        private readonly ILogger<BookTechnicianAddressController> _logger;

        public BookTechnicianAddressController(ICosmosDbService<BookTechnicianAddress> cosmosDbService, ILogger<BookTechnicianAddressController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        [HttpPost("AddressUpload")]
        public async Task<IActionResult> UploadAddress([FromBody] BookTechnicianAddress BookTechnicianAddress)
        {
            if (BookTechnicianAddress == null)
            {
                _logger.LogWarning("BookTechnicianAddress data is null.");
                return BadRequest("BookTechnicianAddress data cannot be null.");
            }

            try
            {
                BookTechnicianAddress.id = Guid.NewGuid().ToString();
                BookTechnicianAddress.BookTechnicianAddressId = Guid.NewGuid().ToString();
                await _cosmosDbService.AddItemAsync(BookTechnicianAddress);  // Add item to Cosmos DB

                _logger.LogInformation("Address uploaded successfully with ID {AddressId}", BookTechnicianAddress.id);

                return Ok(new
                {
                    Message = "Address data uploaded successfully",
                    AddressId = BookTechnicianAddress.BookTechnicianAddressId.ToString()  // Return as string in the response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading address.");
                return StatusCode(500, "An error occurred while uploading the address. Please try again.");
            }
        }


        [HttpGet("GetBookTechnicianAddressById/{userId}")]

        public async Task<ActionResult<BookTechnicianAddress>> GetBookTechnicianAddressById(string userId)

        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId is required.");
            }

            try
            {
                // Fetch the address by UserId from Cosmos DB
                var BookTechnicianAddress = await _cosmosDbService.GetBookTechnicianAddress( userId);
                List<BookTechnicianAddress> addresses = new List<BookTechnicianAddress>();
                foreach (var addres in BookTechnicianAddress)
                {
                    if (addres.IsPrimaryAddress == null)
                    {
                        addres.IsPrimaryAddress = true;

                    }


                    addres.id = addres.id;
                    addresses.Add(addres);
                }
                if (addresses == null)
                {
                    return NotFound($"BookTechnicianAddress with UserId {userId} not found.");
                }

                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching address by UserId.");
                return StatusCode(500, "An error occurred while retrieving the address. Please try again.");
            }
        }
    }
}

