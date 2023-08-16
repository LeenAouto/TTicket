using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.DTOs;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManager _productManager;

        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductManager productManager, ILogger<ProductsController> logger)
        {
            _productManager = productManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productManager.GetAll();
                if (!products.Any())
                    return NotFound($"No products were found");

                return Ok(products);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var product = await _productManager.Get(id);
                if (product == null)
                    return NotFound($"No product with id = {id} was found.");

                return Ok(product);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductBaseDto dto)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest($"Product name is required");

                var product = new Product {
                    Name = dto.Name
                };
                await _productManager.Add(product);
                return Ok(product);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductBaseDto dto)
        {
            try
            {
                var product = await _productManager.Get(id);
                if (product == null)
                    return NotFound($"No product with id = {id} was found.");

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest($"Product name is required");

                product.Name = dto.Name;
                _productManager.Update(product);
                return Ok(product);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var product = await _productManager.Get(id);
                if (product == null)
                    return NotFound($"No product with id = {id} was found.");

                _productManager.Delete(product);
                return Ok(product);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }
    }
}
