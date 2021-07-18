using FrontEndWebApp.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApiHelper _apiHelper;

        public CategoryService(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase<List<Category>>> GetAll()
        {
            var response = await _apiHelper.NonBodyQueryAsync<List<Category>>(HttpMethod.Get, "/api/Categories");
            return response;
        }

        public async Task<ResponseBase<List<Exam>>> GetAllExams(int id)
        {
            var response = await _apiHelper.NonBodyQueryAsync<List<Exam>>(HttpMethod.Get, $"/api/Categories/Exams/{id}");
            return response;
        }
    }
}
