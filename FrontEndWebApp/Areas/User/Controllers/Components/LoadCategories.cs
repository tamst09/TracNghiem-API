using FrontEndWebApp.Areas.User.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndWebApp.Areas.User.Controllers.Components
{
    [ViewComponent(Name = "ListCategories")]
    public class LoadCategories : ViewComponent
    {
        private readonly ICategoryService categoryService;

        public LoadCategories(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await categoryService.GetAll();
            var data = categories.success ? categories.data : new List<TN.Data.Entities.Category>();
            return View("Default", data);
        }
    }
}
