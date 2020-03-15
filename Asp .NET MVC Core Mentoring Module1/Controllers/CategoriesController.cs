using Asp_.NET_Core_Mentoring_Module1.Common.Entities;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Asp_.NET_Core_Mentoring_Module1.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Controllers
{
    [ServiceFilter(typeof(LoggingActionFilter))]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Categories
        public ActionResult Index()
        {
            return View(_unitOfWork.Repository<Categories>().GetAll());
        }
    }
}