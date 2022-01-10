using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TQL.BillableApplication.API.Context;
using TQL.BillableApplication.API.Model.RequestModel;
using TQL.BillableApplication.API.Model.ResponseModel;


namespace TQL.BillableApplication.API.Respository
{
    public class CustomerBillingRepo : ICustomerBillingRepo
    {
        private readonly DapperContext _context;
        private const string StandardDeliveryType= "Standard";
        private const string SpeedDeliveryType = "Speed";
        private const decimal StandardDeliveryCost = 15;
        private const decimal SpeedDeliveryCost = 30;
        public CustomerBillingRepo(DapperContext context)
        {
            _context = context;
        }
        public async Task<ResponseMessage> CreateCustomerBilling(CustomerBilling customerBilling)
        {
            var query = //CustomerBillingQuery.CustomerBillingInsertion;

            "Insert into CustomerBilling values" +
                "(@LastName," +
                "@FirstName," +
                "@Address," +
                "@City," +
                "@PhoneNo," +
                "@FromAddress," +
                "@FromCity," +
                "@FromPostalCode," +
                "@ToAddress," +
                "@ToCity," +
                "@ToPostalCode," +
                "@Commodity," +
                "@Weight," +
                "@DeliveryType" +
                ",@Comments,0,0,0)"+
                "Select cast(Scope_Identity() as int)";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    int poNumber = await connection.QuerySingleAsync<int>(query, customerBilling);
                    if (poNumber > 0)
                    {
                        customerBilling.PONumber = poNumber;
                        if(CalculateShippingCost(customerBilling))
                            return new ResponseMessage() { IsSucess = true, SucessMessage = "Inserted the record successfully", PONumber = poNumber };
                    }
                    return new ResponseMessage() { IsSucess = false, ErrorMessage = "Insertion Falied !!" };
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

        }
        public async Task<CustomerBilling> GetCustomerBillingInvoice(int poNumber)
        {
            var query = CustomerBillingQuery.GetCustomerBillingInfo;
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    CustomerBilling customerBilling = (await connection.QueryAsync<CustomerBilling>(query, new { PONumber = poNumber })).FirstOrDefault();
                    return customerBilling;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool CalculateShippingCost(CustomerBilling customerBilling)
        {
            decimal distance = 50;
            decimal shippingCost = 0;
            decimal tax=0;
            decimal totalCost = 0;
            var query = CustomerBillingQuery.UpdateCost;
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    if (customerBilling.DeliveryType == StandardDeliveryType)
                    {
                        shippingCost = distance * StandardDeliveryCost * customerBilling.Weight;
                    }
                    else if (customerBilling.DeliveryType == SpeedDeliveryType)
                    {
                        shippingCost = distance * SpeedDeliveryCost * customerBilling.Weight;
                    }
                    if (shippingCost <= 20000)
                        tax = .05m * shippingCost;
                    else if (shippingCost > 20000 && shippingCost <= 70000)
                    {
                        tax = .08m * shippingCost;
                    }
                    else
                    {
                        tax = .12m * shippingCost;
                    }
                    totalCost = shippingCost + tax;
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Tax", tax);
                    dynamicParameters.Add("ShippingCost", shippingCost);
                    dynamicParameters.Add("TotalCost", totalCost);
                    dynamicParameters.Add("PONumber", customerBilling.PONumber);
                    int rowCount =  connection.ExecuteAsync(query, dynamicParameters).Result;
                    if (rowCount > 0)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
