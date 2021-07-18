using FrontEndWebApp.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exam;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public class ExamService : IExamService
    {
        private readonly IApiHelper _apiHelper;

        public ExamService(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase> Create(ExamModel model)
        {
            var createResponse = await _apiHelper.CommandAsync(HttpMethod.Post, $"/api/exams", model);
            return createResponse;
        }

        public async Task<ResponseBase> Delete(int id)
        {
            var createResponse = await _apiHelper.NonBodyCommandAsync(HttpMethod.Delete, $"/api/exams?id={id}");
            return createResponse;
        }

        public async Task<ResponseBase<List<Exam>>> GetAll()
        {
            var response = await _apiHelper.NonBodyQueryAsync<List<Exam>>(HttpMethod.Get ,$"/api/Exams");
            return response;
        }

        public async Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model)
        {
            var response = await _apiHelper.QueryAsync<ExamPagingRequest ,PagedResult<Exam>>(HttpMethod.Post,$"/api/Exams/Paged", model);
            return response;
        }

        public async Task<ResponseBase<List<Exam>>> GetOwned()
        {
            var response = await _apiHelper.NonBodyQueryAsync<List<Exam>>(HttpMethod.Get, $"/api/Exams/Owned");
            return response;
        }

        public async Task<ResponseBase<Exam>> GetByID(int id)
        {
            var response = await _apiHelper.NonBodyQueryAsync<Exam>(HttpMethod.Get, $"/api/Exams/{id}");
            return response;
        }

        public async Task<ResponseBase<ExamAttemps>> IncreaseAttemps(int id)
        {
            var response = await _apiHelper.NonBodyQueryAsync<ExamAttemps>(HttpMethod.Post, $"/api/Exams/{id}");
            return response;
        }

        public async Task<ResponseBase> Update(ExamModel model)
        {
            var response = await _apiHelper.CommandAsync(HttpMethod.Put, $"/api/Exams", model);
            return response;
        }
    }
}
