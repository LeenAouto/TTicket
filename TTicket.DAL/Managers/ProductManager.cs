using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.DTOs;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;

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

        public async Task<Product> Get(Guid id)
        {
            try
            {
                return await _context.Product.
                    Where(p => p.Id == id).
                    SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<Product> GetByName(string name)
        {
            try
            {
                return await _context.Product.
                    Where(p => p.Name == name).
                    FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetList(ProductListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;
                return await _context.Product.
                    Where(p => p.Name == model.Name || model.Name == null).
                    Skip(skip).
                    Take(model.PageSize).
                    OrderBy(p => p.Name).
                    ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<Product> Add(Product product)
        {
            try
            {
                await _context.Product.AddAsync(product);
                _context.SaveChanges();
                return product;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
        public Product Update(Product product)
        {
            try
            {
                _context.Product.Update(product);
                _context.SaveChanges();
                return product;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public Product Delete(Product product)
        {
            try
            {
                _context.Product.Remove(product);
                _context.SaveChanges();
                return product;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
