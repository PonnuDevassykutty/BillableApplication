using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TQL.BillableApplication.API.Model.RequestModel;
using TQL.BillableApplication.API.Model.ResponseModel;
using TQL.BillableApplication.API.Respository;

namespace BillableApplication.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class BillableController : ControllerBase
    {
        private readonly ICustomerBillingRepo _customerBillingRepo;
        private ICollection<ValidationResult> results = null;
        public BillableController(ICustomerBillingRepo customerBillingRepo)
        {
            _customerBillingRepo = customerBillingRepo;
        }
        [HttpPut]
        public IActionResult CreateCustomerBilling([FromBody] CustomerBilling customerBilling)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                responseMessage = _customerBillingRepo.CreateCustomerBilling(customerBilling).Result;
            }
            catch(Exception ex)
            {
                return BadRequest (ex.Message);
            }
            return Ok(responseMessage);
        }
        [HttpGet]
        public IActionResult GetCustomerBillingInvoice([FromQuery] int poNumber)
        {
            CustomerBilling customerBilling = new CustomerBilling();
            try
            {
                if (poNumber > 0)
                {
                    customerBilling = _customerBillingRepo.GetCustomerBillingInvoice(poNumber).Result;
                    if(customerBilling ==null)
                    {
                        return BadRequest("Invalid PO number");
                    }
                }
                else
                    return BadRequest("Invalid PO number");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(customerBilling);
        }
        
    }
}
