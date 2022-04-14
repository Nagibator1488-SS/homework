using System.Threading.Tasks;
using AutoMapper;
using homeWork.Core.Interfaces;
using homeWork.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vostok.Logging.Abstractions;

namespace homeWork.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILog log;
        private readonly IMapper mapper;

        public ProductsController(IProductService productService, ILog log, IMapper mapper)
        {
            this.log = log;
            this.mapper = mapper;
            _productService = productService;
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OkObjectResult>> GetProduct(string name)
        {
            try
            {
                var product = await _productService.GetProductAsync(name).ConfigureAwait(false);
                if (product == null)
                    return NotFound();
                return Ok(mapper.Map<ProductModel>(product));
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost("{name}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<CreatedResult>> CreateProduct(string name, int price)
        {
            try
            {
                var product = await _productService.AddProductAsync(name, price).ConfigureAwait(false);
                var location = $"{Request.Path.Value}";

                return Created(location, product);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPut("{name}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OkObjectResult>> UpdatePrice(string name, int newPrice)
        {
            try
            {
                var product = await _productService.UpdateProductPriceAsync(name, newPrice).ConfigureAwait(false);
                return Ok(mapper.Map<ProductModel>(product));
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("{name}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<NoContentResult>> DeleteProduct(string name)
        {
            try
            {
                await _productService.DeleteProductAsync(name);
                return NoContent();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{name}/trybuy")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OkObjectResult>> TryBuyProduct(string name, int moneyAmount)
        {
            try
            {
                var productIsBought = await _productService.TryBuyProductAsync(name, moneyAmount).ConfigureAwait(false);

                return Ok(productIsBought);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
