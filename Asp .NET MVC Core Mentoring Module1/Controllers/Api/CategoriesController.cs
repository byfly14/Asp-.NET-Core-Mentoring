using System;
using System.IO;
using System.Threading.Tasks;
using Asp._NET_Core_Mentoring_Module1.Helpers;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageHelper _imageHelper;
        private readonly ILogger _logger;

        public CategoriesController(IUnitOfWork unitOfWork,
            ILoggerFactory factory, 
            IImageHelper imageHelper)
        {
            _unitOfWork = unitOfWork;
            _imageHelper = imageHelper;
            _logger = factory.CreateLogger(typeof(CategoriesController));
        }

        [HttpGet]
        public IActionResult Get()
        {
            var categories = _unitOfWork.Repository<Categories>().GetAll();
            return Ok(categories);
        }

        [HttpGet("{id}", Name = "GetImage")]
        public async Task<IActionResult> Get(int id)
        {
            return await PerformAction(() => GetCategoryImageActionAsync(id));
        }

        [HttpPost("UploadSingleFile")]
        public async Task<IActionResult> Post(IFormFile file, [FromForm]int id)
        {
            return await PerformAction(async () => await UploadNewCategoryImageActionAsync(id, file));
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

        private async Task<IActionResult> GetCategoryImageActionAsync(int id)
        {
            var category = _unitOfWork.Repository<Categories>().GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var fs = await _imageHelper.CreateImageFileStreamAsync(filePath, category.Picture).ConfigureAwait(false);

            return Ok(fs);
        }

        private async Task<IActionResult> UploadNewCategoryImageActionAsync(int id, IFormFile file)
        {
            var category = _unitOfWork.Repository<Categories>().GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream).ConfigureAwait(false);
                category.Picture = memoryStream.ToArray();
            }

            _unitOfWork.Repository<Categories>().Update(category);
            _unitOfWork.Context.SaveChanges();

            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var fs = _imageHelper.CreateImageFileStream(filePath, category.Picture);

            var uri = Url.Link("GetImage", new { id = category.CategoryId });

            return Created(uri, fs);
        }
    }
}
