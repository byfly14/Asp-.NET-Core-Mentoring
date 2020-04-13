using System.IO;
using System.Threading.Tasks;
using Asp._NET_Core_Mentoring_Module1.Helpers;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Asp_.NET_Core_Mentoring_Module1.Logging;
using Asp_.NET_MVC_Core_Mentoring_Module1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Controllers
{
    [ServiceFilter(typeof(LoggingActionFilter))]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiskImageCacheService _diskImageCacheService;

        public CategoriesController(IUnitOfWork unitOfWork, IDiskImageCacheService diskImageCacheService)
        {
            _unitOfWork = unitOfWork;
            _diskImageCacheService = diskImageCacheService;
        }

        // GET: Categories
        public ViewResult Index()
        {
            return View(_unitOfWork.Repository<Categories>().GetAll());
        }

        [HttpGet]
        public FileStreamResult GetCategoryImage(int id)
        {
            return GetImageFile(id);
        }

        [HttpGet]
        [Route("/attributesRoutingImage/{id:int}")]

        public FileStreamResult GetCategoryImageViaAttributeRouting(int id)
        {
            return GetImageFile(id);
        }
        
        public ActionResult Upload(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile uploadedFile, [FromRoute] int id)
        {
            var category = _unitOfWork.Repository<Categories>().GetById(id);
            
            using (var memoryStream = new MemoryStream())
            {
                uploadedFile.CopyTo(memoryStream);
                category.Picture = memoryStream.ToArray();
            }

            _unitOfWork.Repository<Categories>().Update(category);
            _unitOfWork.Context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private FileStreamResult GetImageFile(int id)
        {
            const string contentType = "image/bmp";
            const string fileName = "Image.bmp";

            FileStream fs;

            if (_diskImageCacheService.TryGetImagePath(id, out var pathToCacheImage))
            {
                fs = new FileStream(pathToCacheImage, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            else
            {
                var category = _unitOfWork.Repository<Categories>().GetById(id);
                fs = GetImageFileStream(category.Picture);
            }
            
            return File(fs, contentType, fileName);
        }

        private static FileStream GetImageFileStream(byte[] bytes)
        {
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            return ImageHelper.CreateImageFileStream(filePath, bytes);
        }
    }
}