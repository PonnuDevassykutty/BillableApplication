using Dapper;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        private const decimal StandardDeliveryCost = 5;
        private const decimal SpeedDeliveryCost = 8;
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
                "@Email,"+
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
                "@DeliveryType," +
                "@PickUpDate,"+
                "@Comments,0,0,0)"+
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
            string origin = string.Concat(customerBilling.FromAddress, customerBilling.FromCity,customerBilling.FromPostalCode);//"Oberoi Mall, Goregaon";
            string destination = string.Concat(customerBilling.ToAddress, customerBilling.ToCity, customerBilling.ToPostalCode);//"Infinity IT Park, Malad East";
            //string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + origin + "&destinations=" + destination + "&key=99f2fd51b9f5ab60251c77d95b4e181c";
            string url = "https://www.zipcodeapi.com/rest/DemoOnly00r7HSe9B2uclbCgzXXfv5bFyfNMb0XAa0oH24g4QIW1QJwzGFUm64ys/distance.json/"+ customerBilling.FromPostalCode + "/"+ customerBilling.ToPostalCode + "/km";
            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    distance = Convert.ToDecimal(reader.ReadToEnd().Split(":")[1].Replace("}", ""));
                }
            }
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
