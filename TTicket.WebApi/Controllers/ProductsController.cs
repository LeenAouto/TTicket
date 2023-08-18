using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.DTOs;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

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

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetAll([FromQuery] ProductListRequestModel model)
        {
            try
            {
                var products = await _productManager.GetList(model);
                if (!products.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.ProductsNotFound,
                        $"No products were not found"));

                return Ok(new Response<IEnumerable<Product>>(products, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetProduct")]
        public async Task<IActionResult> Get([FromQuery] ProductRequestModel model)
        {
            try
            {
                var product = await _productManager.Get(model);
                if (product == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.ProductNotFound,
                        $"No product was not found"));

                return Ok(new Response<Product>(product, ErrorCode.NoError));
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
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidProductName,
                        $"Invalid product name"));

                if(await _productManager.Get(new ProductRequestModel { Name = dto.Name }) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.ProductNameAlreadyUsed,
                        $"Product name is already used"));

                var product = new Product {
                    Name = dto.Name
                };

                await _productManager.Add(product);
                return Ok(new Response<Product>(product, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductBaseDto dto)
        {
            try
            {
                var product = await _productManager.Get(new ProductRequestModel { Id = id});
                if (product == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.ProductNotFound,
                        $"No product was not found"));

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidProductName,
                        $"Invalid product name"));

                if (await _productManager.Get(new ProductRequestModel { Name = dto.Name }) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.ProductNameAlreadyUsed,
                        $"Product name is already used"));

                product.Name = dto.Name;
                _productManager.Update(product);
                return Ok(new Response<Product>(product, ErrorCode.NoError));
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
                var product = await _productManager.Get(new ProductRequestModel { Id = id });
                if (product == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.ProductNotFound,
                        $"No product was not found"));

                _productManager.Delete(product);
                return Ok(new Response<Product>(product, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }
    }
}
