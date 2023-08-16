using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using TTicket.Abstractions.DAL;
using TTicket.Models;

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
                return await _context.Product.SingleOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            try
            {
                return await _context.Product.
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

        public async Task<bool> IsValidProductId(Guid id)
        {
            try
            {
                return await _context.Product.AnyAsync(p => p.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
