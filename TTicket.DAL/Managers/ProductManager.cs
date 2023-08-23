using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.DAL.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductManager> _logger;

        public ProductManager(ILogger<ProductManager> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProductModel> Get(Guid id)
        {
            try
            {
                var product = await _context.Product.
                    Where(p => p.Id == id).
                    Select(p => new ProductModel(p)).
                    SingleOrDefaultAsync();

                return product;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<ProductModel> GetByName(string name)
        {
            try
            {
                var product = await _context.Product.
                    Where(p => p.Name == name).
                    Select(p => new ProductModel(p)).
                    FirstOrDefaultAsync();

                return product;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<PagedResponse<ProductModel>> GetList(ProductListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;

                var query = _context.Product.
                  Where(p => p.Name == model.Name || model.Name == null);

                var totalCount = await query.CountAsync();

                var products = await query.
                  OrderBy(p => p.Name).
                  Skip(skip).
                  Take(model.PageSize).
                  Select(p => new ProductModel(p)).
                  ToListAsync();

                return new PagedResponse<ProductModel>(products, totalCount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<ProductModel> Add(ProductModel product)
        {
            try
            {
                var p = new Product
                {
                    Name = product.Name
                };

                await _context.Product.AddAsync(p);
                _context.SaveChanges();

                return new ProductModel(p);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
        public async Task<ProductModel> Update(ProductModel product)
        {
            try
            {
                var p = await _context.Product.Where(p => p.Id == product.Id).SingleAsync();

                p.Name = product.Name;

                _context.Product.Update(p);
                _context.SaveChanges();

                return new ProductModel(p);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<ProductModel> Delete(ProductModel product)
        {
            try
            {
                var p = await _context.Product.Where(p => p.Id == product.Id).SingleAsync();

                _context.Product.Remove(p);
                _context.SaveChanges();

                return new ProductModel(p);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
