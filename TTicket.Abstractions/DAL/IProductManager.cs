using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.Abstractions.DAL
{
    public interface IProductManager
    {
        Task<ProductModel> Get(Guid id);
        Task<ProductModel> GetByName(string name);
        Task<PagedResponse<ProductModel>> GetList(ProductListRequestModel model);
        Task<ProductModel> Add(ProductModel product);
        Task<ProductModel> Update(ProductModel product);
        Task<ProductModel> Delete(ProductModel product);
    }
}
