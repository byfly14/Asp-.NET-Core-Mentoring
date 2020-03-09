using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asp_.NET_Core_Mentoring_Module1.Controllers
{
    public class CateroriesController : Controller
    {
        // GET: Caterories
        public ActionResult Index()
        {
            return View();
        }

        // GET: Caterories/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Caterories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Caterories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Caterories/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Caterories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Caterories/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Caterories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}