using System;
using System.Collections.Generic;
using System.Linq;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Helpers
{
    public class ViewDataHelper: IViewDataHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Dictionary<Type, List<SelectListItem>> _lists = new Dictionary<Type, List<SelectListItem>>();

        public ViewDataHelper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<SelectListItem> GetSelectListItems<T>() where T : class
        {
            if (_lists.Keys.Contains(typeof(T)))
            {
                return _lists[typeof(T)];
            }
            
            var list = new List<SelectListItem>();
            var items = _unitOfWork.Repository<T>().GetAll().Select(i =>
            {
                switch (i)
                {
                    case Suppliers supplier:
                        return new SelectListItem(supplier.CompanyName, supplier.SupplierId.ToString());
                    case Categories category:
                        return new SelectListItem(category.CategoryName, category.CategoryId.ToString());
                    default:
                        return null;
                }
            });
            list.AddRange(items);

            _lists[typeof(T)] = list;

            return list;
        }
    }

    public interface IViewDataHelper
    {
        List<SelectListItem> GetSelectListItems<T>() where T : class;
    }
}
