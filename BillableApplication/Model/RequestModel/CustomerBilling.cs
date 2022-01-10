using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public string PhoneNo { get; set; }
        [Required]
        public string FromAddress { get; set; }
        [Required]
        public string FromCity { get; set; }
        [Required]
        public string FromPostalCode { get; set; }
        [Required]
        public string ToAddress { get; set; }
        [Required]
        public string ToCity { get; set; }
        [Required]
        public string ToPostalCode { get; set; }
        [Required]
        public string Commodity { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public string DeliveryType { get; set; }
        
        public string Comments { get; set; }
        public int PONumber { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
