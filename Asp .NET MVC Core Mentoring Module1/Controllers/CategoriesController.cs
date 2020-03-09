using System.Drawing;
using System.IO;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Microsoft.AspNetCore.Mvc;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Controllers
{
    public class CategoriesController : Controller
    {
        public NorthWindContext Context { get; }

        public CategoriesController(NorthWindContext context)
        {
            Context = context;
        }

        // GET: Categories
        public ActionResult Index()
        {
            return View(Context.Categories);
        }
        
        //// GET: Categories/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Categories/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Categories/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Categories/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Categories/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Categories/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Categories/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}