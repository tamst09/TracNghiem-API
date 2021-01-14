﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var allCategory = await _categoryService.GetAll();
            return Ok(new ResponseBase<List<Category>>() { data = allCategory });
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetByID(id);
            if (category != null)
            {
                return Ok(new ResponseBase<Category>() { data = category });
            }
            return Ok(new ResponseBase<Category>() { msg = "Category not found" });
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.ID)
            {
                return Ok(new ResponseBase<Category>() { msg = "Invalid category" });
            }
            var updateResult = await _categoryService.Update(category);
            if (updateResult == null)
            {
                return Ok(new ResponseBase<Category>() { msg = "Update failed" });
            }
            return Ok(new ResponseBase<Category>() { data = updateResult });
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> PostCategory([Bind("CategoryName")]Category category)
        {
            var createResult = await _categoryService.Create(category);
            return Ok(new ResponseBase<Category>() { data = createResult });
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleteResult = await _categoryService.Delete(id);
            if (deleteResult)
                return Ok(new ResponseBase<Category>() { });
            else
                return Ok(new ResponseBase<Category>() { msg = "Delete failed" });
        }

        // DELETE: api/Categories/DeleteRange
        [HttpPost("DeleteRange")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteManyCategory(DeleteRangeModel<int> lstCategoryId)
        {
            var deleteResult = await _categoryService.DeleteListCategory(lstCategoryId);
            if (deleteResult)
                return Ok(new ResponseBase<Category>() { });
            else
                return Ok(new ResponseBase<Category>() { msg = "Delete failed" });
        }
    }
}
