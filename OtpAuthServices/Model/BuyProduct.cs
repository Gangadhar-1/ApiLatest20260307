using Microsoft.Azure.Cosmos.Core.Networking;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OtpAuthServices.Model
{
    public class BuyProduct
    { 
        public  string  id    { get; set; }
         
        public  string  BuyProductId { get; set; }
        [Required]
        [StringLength(250, ErrorMessage = "Address is too long.")]
        public string Address { get; set; }

        [Required]
        public string Category         { get; set; }
        public string  DeliveryDate { get; set; }   


        public string TechnicianDetils { get; set; }        


        public string InvoiceDetails { get; set; }          


       public string AssignedTo { get; set; }   
        public string status { get; set; }    
        [Required]
        public string ProductName      { get; set; }

        public string ProductCatalogue  { get; set; }
        public string ProductSize       { get; set; }
        public string Color { get; set; }


        public string rate { get; set; }    
        
        public string discount { get; set; }    

        public  string afterDiscountPrice { get; set; }


       
     public string   selectedColors { get; set; }
        // otherThanProduct,

        public string totalAmount { get; set; } 


        // public string OtherThanProduct { get; set; }
        public string RequiredQuantity { get; set; }
       // public string Units { get; set; }
        public string AddressType { get; set; }

        [Required]
        public string State { get; set; }



        [Required]
        public string District { get; set; }

        //[Required]
        //[RegularExpression("^[0-9]{6}$", ErrorMessage = "PinCode must be a 6-digit number.")]
        public string ZipCode { get; set; }

        public string  CustomerId { get; set; } 


        public string CustomerName { get; set; }     

        public string CustomerPhoneNumber { get; set; }

        public string PaymentMode { get; set; }

        // string ApprovedAmount { get; set; }
        public string UTRTransactionNumber { get; set; }


        public string TechnicianConfirmationCode { get; set; }





        public string DeliveryCharges { get;  set; }  

        public string ServiceCharges { get; set; }      


        public string TotalPaymentAmount { get; set; }


    }
}
