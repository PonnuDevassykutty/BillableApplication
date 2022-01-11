using System;
using System.ComponentModel.DataAnnotations;

namespace TQL.BillableApplication.API.Model.RequestModel
{
    public class CustomerBilling
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public int PhoneNo { get; set; }
        
        public string Email { get; set; }
        [Required]
        public string FromAddress { get; set; }
        [Required]
        public string FromCity { get; set; }
        [Required]
        public int FromPostalCode { get; set; }
        [Required]
        public string ToAddress { get; set; }
        [Required]
        public string ToCity { get; set; }
        [Required]
        public int ToPostalCode { get; set; }
        [Required]
        public string Commodity { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public string DeliveryType { get; set; }
        [Required]
        public DateTime PickUpDate { get; set; }
        public string Comments { get; set; }
        public int PONumber { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
