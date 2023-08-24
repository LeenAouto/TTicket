namespace TTicket.Models.PresentationModels.DashboardModels
{
    public class ProductsTicketsModel
    {
        public string ProductName { get; set; } = string.Empty;
        public List<CountersModel> Counters { get; set; } = new List<CountersModel>();
    }
}
