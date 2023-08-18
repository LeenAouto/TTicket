namespace TTicket.Models.RequestModels
{
    public class BaseListRequestModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = int.MaxValue;
    }
}
