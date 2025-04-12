using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;
using OtpAuthServices.Models;
using OtpAuthServices.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace OtpAuthServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestUserController : ControllerBase
    {
        private readonly ICosmosDbService<GuestUser> _cosmosDbService;
        public GuestUserController(ICosmosDbService<GuestUser> cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;

        }

        [HttpPost("UploadGuestUser")]
        public async Task<IActionResult> UploadGuestUser([FromBody] GuestUser GuestUser)
        {
            if (GuestUser == null)
            {
                return BadRequest("GuestUser data cannot be null.");
            }
            GuestUser.UserId = Guid.NewGuid().ToString();

            GuestUser.DateTime = DateTime.UtcNow;
            GuestUser.id = Guid.NewGuid().ToString();

            await _cosmosDbService.AddItemAsync(GuestUser);
            return Ok(new { Message = "GuestUser Data Uploaded successfully", GuestUserId = GuestUser.UserId });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuestUser(string id)
        {
            var existingaddress = await _cosmosDbService.GetItemAsync(id);
            if (existingaddress == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync(id);
            return Ok("Successfully  deleted GuestUser Data   Item. ");
        }

        
        [HttpGet("GuestUser/{id}")]
        public async Task<IActionResult> GetBookTechnician(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("GuestUser ID cannot be null or empty.");
            }

            var bookTechnician = await _cosmosDbService.GetItemAsync(id);
            if (bookTechnician == null)
            {
                return NotFound($"GuestUser with ID {id} not found.");
            }

            return Ok(bookTechnician);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuestUser(string id, [FromBody] GuestUser GuestUser)
        {
            if (GuestUser == null || GuestUser.id != id)
            {
                return BadRequest("GuestUser information is incorrect.");
            }

            var existingaddress = await _cosmosDbService.GetItemAsync(id);
            if (existingaddress == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(GuestUser);
            return Ok("GuestUser data Updated successfully.");
        }



    }
}