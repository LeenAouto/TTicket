using TTicket.Models;
using TTicket.Models.RequestModels;

namespace TTicket.Abstractions.DAL
{
    public interface IProductManager
    {
        Task<Product> Get(Guid id);
        Task<Product> GetByName(string name);
        Task<IEnumerable<Product>> GetList(ProductListRequestModel model);
        Task<Product> Add(Product product);
        Product Update(Product product);
        Product Delete(Product product);
    }
}
