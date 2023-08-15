﻿using TTicket.Models;

namespace TTicket.DAL.Interfaces
{
    public interface IProductManager
    {
        Task<Product> Get(Guid id);
        Task<IEnumerable<Product>> GetAll();
        Task<Product> Add(Product product);
        Product Update(Product product);
        Product Delete(Product product);
        Task<bool> IsValidProductId(Guid id);
    }
}
