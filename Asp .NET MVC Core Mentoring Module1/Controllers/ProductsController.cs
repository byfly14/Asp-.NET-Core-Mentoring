using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Asp._NET_Core_Mentoring_Module1.Helpers;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Asp_.NET_Core_Mentoring_Module1.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Controllers
{
    [ServiceFilter(typeof(LoggingActionFilter))]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pagesPerPage;
        private readonly Expression<Func<Products, object>>[] _includeProperties = { p => p.Category, p => p.Supplier };
        public int TotalCount { get; }

        public ProductsController(IUnitOfWork unitOfWork,
                                  IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            int.TryParse(config["ItemsPerPage"], out _pagesPerPage);
            _pagesPerPage = _pagesPerPage < 0 ? 0 : _pagesPerPage;

            TotalCount = _unitOfWork.Repository<Products>().Count();
        }

        // GET: Products
        public ActionResult Index()
        {
            var viewData = _unitOfWork.Repository<Products>()
                .GetAllWithInclude(_includeProperties)
                .Take(_pagesPerPage == 0 ? TotalCount : _pagesPerPage);
            return View(viewData);
        }

        // GET: Products/Details/5
        public ActionResult Details(int id)
        {
            var idPredicate = new Func<Products, bool>(p => p.ProductId == id);
            var viewData = _unitOfWork.Repository<Products>().GetByIdWithInclude(idPredicate, _includeProperties);
            return View(viewData);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var (categories, suppliers) = GetViewBagData();
            ViewBag.Categories = categories;
            ViewBag.Suppliers = suppliers;
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Products product)
        {
            void CreateAction()
            {
                _unitOfWork.Repository<Products>().Create(product);
                _unitOfWork.Commit();
            }

            if (!TransactionHelper.TryPerformTransaction(_unitOfWork.Context, CreateAction, out var exception))
            {
                throw exception;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {
            var (categories, suppliers) = GetViewBagData();
            ViewBag.Categories = categories;
            ViewBag.Suppliers = suppliers;
            var viewData = _unitOfWork.Repository<Products>().GetById(id);
            return View(viewData);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Products product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            void UpdateAction()
            {
                _unitOfWork.Repository<Products>().Update(product);
                _unitOfWork.Commit();
            }

            if (!TransactionHelper.TryPerformTransaction(_unitOfWork.Context, UpdateAction, out var exception))
            {
                throw exception;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int id)
        {
            var idPredicate = new Func<Products, bool>(p => p.ProductId == id);
            var viewData = _unitOfWork.Repository<Products>().GetByIdWithInclude(idPredicate, _includeProperties);
            return View(viewData);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([FromForm] Products product)
        {
            void DeleteAction()
            {
                _unitOfWork.Repository<Products>().Delete(product);
                _unitOfWork.Commit();
            }

            if (!TransactionHelper.TryPerformTransaction(_unitOfWork.Context, DeleteAction, out var exception))
            {
                throw exception;
            }

            return RedirectToAction(nameof(Index));
        }

        private (List<SelectListItem> categories, List<SelectListItem> suppliers) GetViewBagData()
        {
            return (new List<SelectListItem>(_unitOfWork.Repository<Categories>()
                        .GetAll()
                        .Select(c => new SelectListItem(c.CategoryName, c.CategoryId.ToString()))),
                    new List<SelectListItem>(_unitOfWork.Repository<Suppliers>()
                        .GetAll()
                        .Select(c => new SelectListItem(c.CompanyName, c.SupplierId.ToString()))));

        }
    }
}