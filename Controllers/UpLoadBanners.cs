using Azure.Storage.Blobs.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.Azure.Cosmos;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;
using OtpAuthServices.Models;
using OtpAuthServices.Services;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace OtpAuthServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpLoadBannners : Controller
    {
        private readonly ICosmosDbService<UploadBanners> _cosmosDbService;
        private readonly ILogger<UpLoadBannners> _logger;

        public UpLoadBannners(ICosmosDbService<UploadBanners> cosmosDbService, ILogger<UpLoadBannners> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        [HttpPost("UploadBanners")]
        public async Task<IActionResult> UploadBanners([FromBody] UploadBanners uploadBanners)
        {
            if (uploadBanners == null)
            {
                return BadRequest("uploadBanners cannot be null");
            }

            try
            {
                string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? "India Standard Time"
    : "Asia/Kolkata";

                TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone);


                uploadBanners.Date = indianTime.ToString("hh:mm tt dd-MM-yyyy");

                uploadBanners.id = Guid.NewGuid().ToString();

                uploadBanners.BannerId = Guid.NewGuid().ToString();

                await _cosmosDbService.AddItemAsync(uploadBanners);

                return Ok(new
                {
                    message = "uploadBanners uploaded successfully.",
                    id = uploadBanners.id,

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading uploadBanners.");
                return StatusCode(500, "An error occurred while uploading the uploadBanners. Please try again later.");
            }

        }

        
        
        [HttpGet("GetBanners")]

        public async Task<IActionResult> GetBanners()
        {

            var banners = await _cosmosDbService.GetBanners();

            if (banners == null)
            {
                return NotFound("Get Banners can not be found");
            }

            return Ok(banners);
        }



    }
    
}
