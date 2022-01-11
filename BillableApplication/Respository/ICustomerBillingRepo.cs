using System.Threading.Tasks;
using TQL.BillableApplication.API.Model.RequestModel;
using TQL.BillableApplication.API.Model.ResponseModel;

namespace TQL.BillableApplication.API.Respository
{
    public interface ICustomerBillingRepo
    {
        public Task<ResponseMessage> CreateCustomerBilling(CustomerBilling customerBilling);
        public Task<CustomerBilling> GetCustomerBillingInvoice(int poNumber);
    }
}
