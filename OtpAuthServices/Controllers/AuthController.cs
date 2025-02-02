using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Caching.Memory;

using Microsoft.Extensions.Options;
using static System.Net.WebRequestMethods;
using Microsoft.Identity.Client;
using System.Collections.Specialized;
using System.Web;
using System.Text;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;
using Twilio.TwiML.Messaging;

namespace OtpAuthServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TwilioSettings _twilioSettings;
        private readonly IMemoryCache _memoryCache;
        private string? LSSPHM;
        private readonly ICosmosDbService<UserOnBoarding> _cosmosDbService;

        public AuthController(IMemoryCache cache,IConfiguration configuration, IOptions<TwilioSettings> twilioSettings, ICosmosDbService<UserOnBoarding> cosmosDbService)
        {
            _memoryCache = cache;
            _configuration = configuration;
            _twilioSettings = twilioSettings.Value;
            _cosmosDbService = cosmosDbService;
        }

        //[HttpPost("generate-otp")]
        //public async Task<IActionResult> GenerateOtp([FromBody] OtpRequest request)
        //{

        //    // Generate a 6-digit OTP
        //    string otp = GenerateRandomOtp();

        //    try
        //    {
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //        {

        //            using (var command = new SqlCommand("Usp_User_Otp", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@PhoneNumber", request.PhoneNumber);
        //                command.Parameters.AddWithValue("@Email", request.Email);
        //                //command.Parameters.AddWithValue("@OTP", otp);

        //                await command.ExecuteNonQueryAsync();
        //            }
        //        }

        //        //return Ok(new { Message = "OTP generated successfully.", OTP = otp });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal server error: " + ex.Message);
        //    }
        //}



        [HttpPost("sendotp")]
        public async Task<IActionResult> SendOtp([FromBody] OptRequest request)
        {
          

            string dynmaicotp = GenerateRandomOtp();

            if (request.Type == "email")
            {
                if (string.IsNullOrEmpty(request.SenderValue))
                {
                    return Ok("Email and OTP are required.");
                }

                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var fromEmail = smtpSettings["FromEmail"];
                var host = smtpSettings["Host"];
                var port = int.Parse(smtpSettings["Port"]);
                var username = smtpSettings["UserName"];
                var password = smtpSettings["Password"];

                try
                {
                    var smtpClient = new SmtpClient(host)
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(username, password),
                        EnableSsl = true,
                        UseDefaultCredentials = false
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = "Your single-use code",
                        Body = $"Hi {request.SenderValue},\n\nYour OTP is: {dynmaicotp}. It's valid for 3 minutes.",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(request.SenderValue);

                    await smtpClient.SendMailAsync(mailMessage);

                    // Cache OTP with expiration (e.g., 3 minutes)
                    _memoryCache.Set(request.SenderValue, dynmaicotp, TimeSpan.FromMinutes(3));

                    return Ok(new { Message = "OTP email sent successfully." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }
            else if (request.Type == "sms")
            {
                try
                {
                    //TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

                    //var message = MessageResource.Create(
                    //    body: $"Dear Applicant, your OTP is {dynmaicotp}. It's valid for 3 minutes.",
                    //    from: new PhoneNumber(_twilioSettings.FromPhoneNumber),
                    //    to: new PhoneNumber(request.SenderValue)
                    //);

                    //if (message.ErrorCode == null)
                    //{
                    //    // Cache OTP with expiration (e.g., 3 minutes)
                    //    _memoryCache.Set(request.SenderValue, dynmaicotp, TimeSpan.FromMinutes(3));
                    //    return Ok(new { Message = "OTP SMS sent successfully." });
                    //}
                    //else
                    //{
                    //    return Ok(new { success = false, error = message.ErrorMessage });
                    //}


                    //string dynamicOtp = GenerateRandomOtp(); // Assuming this function generates your OTP
                    //string message = HttpUtility.UrlEncode($"Use Verification code {dynmaicotp} for HandyMan Authentication\n\nThanks\nHandy Man Service Providers\nhttps://handymanserviceproviders.com/");
                    // string dynamicOtp = GenerateRandomOtp(); // Assuming this function generates your OTP
                    string result;
                    string apiKey = "NTgzNDZjNzY3MjQ5NDI0YTMxNTE0ZjRlNjQ2MjY0NDU=";
                    string numbers = request.SenderValue;
                    //string dynamicOtp = GenerateRandomOtp();
                    string message = $"Use Verification code {dynmaicotp} for HandyMan Authentication\r\n \r\nThanks\r\nHandy Man Service Providers\r\n https://handymanserviceproviders.com/";
                    string sender = "LSSPHM";

                    // URL encode the message
                    string encodedMessage = WebUtility.UrlEncode(message);

                    string postData = $"apikey={apiKey}&numbers={numbers}&message={encodedMessage}&sender={sender}";

                    // Create the HTTP request
                    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://api.textlocal.in/send/");
                    objRequest.Method = "POST";
                    objRequest.ContentType = "application/x-www-form-urlencoded";
                    objRequest.ContentLength = Encoding.UTF8.GetByteCount(postData);


                    // Write the post data to the request stream
                    using (StreamWriter writer = new StreamWriter(objRequest.GetRequestStream()))
                    {
                        writer.Write(postData);
                    }

                    // Get the response from the server
                    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                    using (StreamReader reader = new StreamReader(objResponse.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                    _memoryCache.Set(request.SenderValue, dynmaicotp, TimeSpan.FromMinutes(3));

                    return Ok(new { Message = "OTP Mobile sent successfully." });

                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

            return Ok("Invalid OTP request.");
        }

        /// <summary>
        /// verify users name already exist or not 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("verifyuserexist")]
        public async Task<IActionResult> VerifyUserExist([FromBody] VerifyUserExist request)
        {
            try
            {
                var user = await _cosmosDbService.GetUserByUserIdAsync(request.UserName);

                if (user != null)
                {
                    // User already exists, return a message
                    return Ok(new { message = "Username already exists, choose another username." });
                }

                // User does not exist, return no content
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the error (using your preferred logging approach)
                Console.WriteLine($"Error in VerifyUserExist: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while verifying the user." });
            }
        }



        /// <summary>
        /// send otp before to check email and mobile already exist with another user or not 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("sendotpbefore")]
        public async Task<IActionResult> sendotpbefore([FromBody] OptRequest request)
        {
            var user = await _cosmosDbService.GetUserByEmailOrMobileAsync(request.SenderValue);

            if (user != null)
            {
                return Ok(new { message = "Email or Mobile Number already exists, choose another email or mobile." });
            
            }

            string dynmaicotp = GenerateRandomOtp();

            if (request.Type == "email")
            {
                if (string.IsNullOrEmpty(request.SenderValue))
                {
                    return Ok(new { Message=  "Email and OTP are required." });
                }

                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var fromEmail = smtpSettings["FromEmail"];
                var host = smtpSettings["Host"];
                var port = int.Parse(smtpSettings["Port"]);
                var username = smtpSettings["UserName"];
                var password = smtpSettings["Password"];

                try
                {
                    var smtpClient = new SmtpClient(host)
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(username, password),
                        EnableSsl = true,
                        UseDefaultCredentials = false
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = "Your single-use code",
                        Body = $"Hi {request.SenderValue},\n\nYour OTP is: {dynmaicotp}. It's valid for 3 minutes.",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(request.SenderValue);

                    await smtpClient.SendMailAsync(mailMessage);

                    // Cache OTP with expiration (e.g., 3 minutes)
                    _memoryCache.Set(request.SenderValue, dynmaicotp, TimeSpan.FromMinutes(3));

                    return Ok(new { Message = "OTP email sent successfully." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }
            else if (request.Type == "sms")
            {
                try
                {
                    //TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

                    //var message = MessageResource.Create(
                    //    body: $"Dear Applicant, your OTP is {dynmaicotp}. It's valid for 3 minutes.",
                    //    from: new PhoneNumber(_twilioSettings.FromPhoneNumber),
                    //    to: new PhoneNumber(request.SenderValue)
                    //);

                    //if (message.ErrorCode == null)
                    //{
                    //    // Cache OTP with expiration (e.g., 3 minutes)
                    //    _memoryCache.Set(request.SenderValue, dynmaicotp, TimeSpan.FromMinutes(3));
                    //    return Ok(new { Message = "OTP SMS sent successfully." });
                    //}
                    //else
                    //{
                    //    return Ok(new { success = false, error = message.ErrorMessage });
                    //}


                    //string dynamicOtp = GenerateRandomOtp(); // Assuming this function generates your OTP
                    //string message = HttpUtility.UrlEncode($"Use Verification code {dynmaicotp} for HandyMan Authentication\n\nThanks\nHandy Man Service Providers\nhttps://handymanserviceproviders.com/");
                    // string dynamicOtp = GenerateRandomOtp(); // Assuming this function generates your OTP
                    string result;
                    string apiKey = "NTgzNDZjNzY3MjQ5NDI0YTMxNTE0ZjRlNjQ2MjY0NDU=";
                    string numbers = request.SenderValue;
                    //string dynamicOtp = GenerateRandomOtp();
                    string message = $"Use Verification code {dynmaicotp} for HandyMan Authentication\r\n \r\nThanks\r\nHandy Man Service Providers\r\n https://handymanserviceproviders.com/";
                    string sender = "LSSPHM";

                    // URL encode the message
                    string encodedMessage = WebUtility.UrlEncode(message);

                    string postData = $"apikey={apiKey}&numbers={numbers}&message={encodedMessage}&sender={sender}";

                    // Create the HTTP request
                    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://api.textlocal.in/send/");
                    objRequest.Method = "POST";
                    objRequest.ContentType = "application/x-www-form-urlencoded";
                    objRequest.ContentLength = Encoding.UTF8.GetByteCount(postData);


                    // Write the post data to the request stream
                    using (StreamWriter writer = new StreamWriter(objRequest.GetRequestStream()))
                    {
                        writer.Write(postData);
                    }

                    // Get the response from the server
                    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                    using (StreamReader reader = new StreamReader(objResponse.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                    _memoryCache.Set(request.SenderValue, dynmaicotp, TimeSpan.FromMinutes(3));

                    return Ok(new { Message = "OTP SMS sent successfully." });

                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

            return Ok("Invalid OTP request.");
        }




        [HttpPost("validateotp")]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)
        {
           
            if (string.IsNullOrEmpty(request.SenderValue) || string.IsNullOrEmpty(request.Otp))
            {
                //return Ok("Sender value and OTP are required.");
                return Ok(new { Message = "Sender value and OTP are required." });
            }

            // Check OTP from cache
            if (_memoryCache.TryGetValue(request.SenderValue, out string cachedOtp))
            {
                if (cachedOtp == request.Otp)
                {
                    // OTP is valid, you can now proceed with further logic
                    _memoryCache.Remove(request.SenderValue); // Optionally remove OTP after successful validation

                    // Use await here since the method is now async
                    //var user = await _cosmosDbService.GetUserByEmailOrMobileAsync(request.SenderValue);

                  
                   
                  return Ok(new { Message = "OTP validated successfully." });
                    
                }
                else
                {
                    return Ok("Invalid OTP.");
                }
            }
            else
            {
                return Ok("OTP expired or not found.");
            }
        }




        [HttpPost("validateFirstOtp")]
        public async Task<IActionResult> validateFirstOtp([FromBody] OtpValidationRequest request)
        {
            if (string.IsNullOrEmpty(request.SenderValue) || string.IsNullOrEmpty(request.Otp))
            {
                return Ok("Sender value and OTP are required.");
            }

            // Check OTP from cache
            if (_memoryCache.TryGetValue(request.SenderValue, out string cachedOtp))
            {
                if (cachedOtp == request.Otp)
                {
                    // OTP is valid, you can now proceed with further logic
                    _memoryCache.Remove(request.SenderValue); // Optionally remove OTP after successful validation

                    // Use await here since the method is now async
                    //var user = await _cosmosDbService.GetUserByEmailOrMobileAsync(request.SenderValue);

                    //if (user != null)
                    //{
                    //    return Ok("Email or Mobile Number already exists, choose another email or mobile.");
                    //}
                    //else
                    //{
                    return Ok(new { Message = "OTP validated successfully." });
                    //}
                }
                else
                {
                    return Ok("Invalid OTP.");
                }
            }
            else
            {
                return Ok("OTP expired or not found.");
            }
        }





        private string GenerateRandomOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }

    public class OtpValidationRequest
    {
        public string SenderValue { get; set; } // email or mobile
       public string Otp { get; set; }
    }
    public class OtpRequest
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class OtpValidateUser
    {
        public string UserId { get; set; }
    }

    public class OtpValidateRequest
    {
        public string PhoneNumber { get; set; }
    }
}


