using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models.DTOs;
using TTicket.Models.PresentationModels;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductListRequestModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var products = await _productManager.GetList(model);
                if (!products.Items.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.ProductsNotFound,
                        $"No products were not found"));

                return Ok(new Response<PagedResponse<ProductModel>>(products, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var product = await _productManager.Get(id);
                if (product == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.ProductNotFound,
                        $"No product was not found"));

                return Ok(new Response<ProductModel>(product, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [NonAction]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductDto dto)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidProductName,
                        $"Invalid product name"));

                if(await _productManager.GetByName(dto.Name) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.ProductNameAlreadyUsed,
                        $"Product name is already used"));

                var product = new ProductModel
                {
                    Name = dto.Name
                };

                var result = await _productManager.Add(product);
                return Ok(new Response<ProductModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }
        
        [NonAction]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto dto)
        {
            try
            {
                var product = await _productManager.Get(id);
                if (product == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.ProductNotFound,
                        $"No product was not found"));

                if(dto.Name != null)
                {
                    if (string.IsNullOrWhiteSpace(dto.Name))
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.InvalidProductName,
                            $"Invalid product name"));

                    if (dto.Name != product.Name && await _productManager.GetByName(dto.Name) != null)
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.ProductNameAlreadyUsed,
                            $"Product name is already used"));

                    product.Name = dto.Name;
                }

                var result = await _productManager.Update(product);
                return Ok(new Response<ProductModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }


        [NonAction]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var product = await _productManager.Get(id);
                if (product == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.ProductNotFound,
                        $"No product was not found"));

                var result = await _productManager.Delete(product);
                return Ok(new Response<ProductModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }
    }
}
