namespace TTicket.Models.ResponseModels
{
    public class PagedResponse<T>
    {
        public PagedResponse(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
