using System;
using System.Threading.Tasks;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ProductsController(IUnitOfWork unitOfWork, ILoggerFactory factory)
        {
            _unitOfWork = unitOfWork;
            _logger = factory.CreateLogger(typeof(ProductsController));
        }

        [HttpGet]
        public IActionResult Get()
        {
            var products = _unitOfWork.Repository<Products>().GetAll();
            return Ok(products);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get(int id)
        {
            return await PerformAction(async () => await GetProductActionAsync(id));
        }

        [HttpPost("CreateNewProduct")]
        public async Task<IActionResult> Post([FromBody] Products product)
        {
            return await PerformAction(async () => await CreateProductActionAsync(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Products product)
        {
            return await PerformAction(async () => await UpdateProductActionAsync(id, product));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await PerformAction(async () => await DeleteProductActionAsync(id));
        }

        private async Task<IActionResult> PerformAction(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong", ex);
            }

            return BadRequest();
        }

        private async Task<IActionResult> GetProductActionAsync(int id)
        {
            var category = _unitOfWork.Repository<Categories>().GetById(id);

            if (category == null)
            {
                return await Task.FromResult(NotFound());
            }
            
            return await Task.FromResult(Ok(category));
        }

        private async Task<IActionResult> CreateProductActionAsync(Products product)
        {
            _unitOfWork.Repository<Products>().Create(product);
            _unitOfWork.Commit();

            var uri = CreateUri("GetProduct", new {id = product.ProductId});

            return await Task.FromResult(Created(uri, product));
        }

        private async Task<IActionResult> UpdateProductActionAsync(int id, Products newProduct)
        {
            var product = _unitOfWork.Repository<Products>().GetById(id);
            if (product == null)
            {
                return await Task.FromResult(NotFound(id));
            }

            UpdateProduct(newProduct, product);

            _unitOfWork.Repository<Products>().Update(product);
            _unitOfWork.Commit();

            var uri = CreateUri("GetProduct", new { id = product.ProductId });

            return await Task.FromResult(Created(uri, product));
        }

        private async Task<IActionResult> DeleteProductActionAsync(int id)
        {
            var product = _unitOfWork.Repository<Products>().GetById(id);

            if (product == null)
            {
                return await Task.FromResult(NotFound(id));
            }

            _unitOfWork.Repository<Products>().Delete(product);
            _unitOfWork.Commit();

            return await Task.FromResult(Ok());
        }

        private string CreateUri(string routeName, object arg)
        {
            return Url.Link(routeName, arg);
        }

        private static void UpdateProduct(Products newProduct, Products oldProduct)
        {
            if (newProduct.CategoryId != null)
            {
                oldProduct.CategoryId = newProduct.CategoryId;
            }

            if (newProduct.ProductName != null)
            {
                oldProduct.ProductName = newProduct.ProductName;
            }

            if (newProduct.QuantityPerUnit != null)
            {
                oldProduct.QuantityPerUnit = newProduct.QuantityPerUnit;
            }

            if (newProduct.ReorderLevel != null)
            {
                oldProduct.ReorderLevel = newProduct.ReorderLevel;
            }

            if (newProduct.SupplierId != null)
            {
                oldProduct.SupplierId = newProduct.SupplierId;
            }

            if (newProduct.UnitPrice != null)
            {
                oldProduct.UnitPrice = newProduct.UnitPrice;
            }

            if (newProduct.UnitsInStock != null)
            {
                oldProduct.UnitsInStock = newProduct.UnitsInStock;
            }

            if (newProduct.UnitsOnOrder != null)
            {
                oldProduct.UnitsOnOrder = newProduct.UnitsOnOrder;
            }
        }
    }
}
