namespace TQL.BillableApplication.API.Model.ResponseModel
{
    public class ResponseMessage
    {
        public string SucessMessage {get; set;} 
        public bool IsSucess {get;set;}
        public string ErrorMessage { get; set; }
        public int PONumber { get; set; }

    }
}
