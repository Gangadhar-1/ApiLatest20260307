using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OtpAuthServices.Model
{
    public class BuyProduct
    { 
        public  string  id { get; set; }
         
        public  string  BuyProductId { get; set; }
        [Required]
        [StringLength(250, ErrorMessage = "Address is too long.")]
        public string Address { get; set; }

        [Required]
        public string Category         { get; set; }

        public string status { get; set; }    
        [Required]
        public string ProductName      { get; set; }

        public string ProductCatalogue  { get; set; }
        public string ProductSize       { get; set; }
        public string Color { get; set; }
        public string OtherThanProduct { get; set; }
        public string RequiredQuantity { get; set; }
        public string Units { get; set; }
        public string AddressType { get; set; }

        [Required]
        public string State { get; set; }



        [Required]
        public string District { get; set; }

        [Required]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "PinCode must be a 6-digit number.")]
        public string ZipCode { get; set; }

        public string  CustomerId { get; set; } 
    }
}
