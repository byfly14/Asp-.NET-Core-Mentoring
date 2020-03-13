using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly int _pagesPerPage;
        private readonly IRepository<Products> _productsRepository;

        private readonly Expression<Func<Products, object>>[] _includeProperties = { p => p.Category, p => p.Supplier };
        public int TotalCount { get; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<SelectListItem> Suppliers { get; set; }

        public ProductsController(IRepository<Products> productsRepository, 
                                  IRepository<Categories> categoriesRepository, 
                                  IRepository<Suppliers> suppliersRepository, 
                                  IConfiguration config)
        {
            _productsRepository = productsRepository;
            int.TryParse(config["ItemsPerPage"], out _pagesPerPage);
            _pagesPerPage = _pagesPerPage < 0 ? 0 : _pagesPerPage;

            TotalCount = _productsRepository.Count();
            Categories = new List<SelectListItem>(categoriesRepository.GetAll().Select(c => new SelectListItem(c.CategoryName, c.CategoryId.ToString())));
            Suppliers = new List<SelectListItem>(suppliersRepository.GetAll().Select(c => new SelectListItem(c.CompanyName, c.SupplierId.ToString())));
        }

        // GET: Products
        public ActionResult Index()
        {
            return View(_productsRepository
                .GetAllWithInclude(_includeProperties)
                .Take(_pagesPerPage == 0 ? TotalCount : _pagesPerPage));
        }

        // GET: Products/Details/5
        public ActionResult Details(int id)
        {
            var idPredicate = new Func<Products, bool>(p => p.ProductId == id);
            return View(_productsRepository.GetByIdWithInclude(idPredicate, _includeProperties));
        }

        //// GET: Products/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Products/Create
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

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Categories = Categories;
            ViewBag.Suppliers = Suppliers;
            return View(_productsRepository.GetById(id));
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Products product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _productsRepository.Update(product);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //// GET: Products/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Products/Delete/5
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