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
            return Ok(new { Message = "GuestUser Data Uploaded successfully", GuestUserId= GuestUser.UserId });
        }
    }

}