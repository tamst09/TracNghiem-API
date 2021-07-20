using FrontEndWebApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exam;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class ExamManage : IExamManage
    {
        private readonly IApiHelper _apiHelper;

        public ExamManage(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase<List<Exam>>> GetAll()
        {
            var res = await _apiHelper.NonBodyQueryAsync<List<Exam>>(HttpMethod.Get, "/api/Exams/Admin");
            return res;
        }

        public async Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model)
        {
            var res = await _apiHelper.QueryAsync<ExamPagingRequest, PagedResult<Exam>>(HttpMethod.Post, "/api/Exams/Admin/Paged", model);
            return res;
        }

        public async Task<ResponseBase<Exam>> GetByID(int id)
        {
            var res = await _apiHelper.NonBodyQueryAsync<Exam>(HttpMethod.Get, $"/api/Exams/Admin/{id}");
            return res;
        }

        public async Task<ResponseBase> Update(ExamModel model)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Put, $"/api/Exams/Admin", model);
            return res;
        }

        public async Task<ResponseBase> Delete(int id)
        {
            var res = await _apiHelper.NonBodyCommandAsync(HttpMethod.Delete, $"/api/Exams/Admin/{id}");
            return res;
        }

        public async Task<ResponseBase> DeleteMany(DeleteManyModel<int> lstId)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Post, $"/api/Exams/Admin/DeleteMany", lstId);
            return res;
        }

        public async Task<ResponseBase> Create(ExamModel model)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Post, $"/api/Exams", model);
            return res;
        }

        public async Task<ResponseBase<ExamAttemps>> IncreaseAttemps(int id)
        {
            var res = await _apiHelper.NonBodyQueryAsync<ExamAttemps>(HttpMethod.Post, $"/api/Exams/IncreaseAttemp/{id}");
            return res;
        }

        public async Task<ResponseBase<List<Exam>>> GetByCategory(int categoryId)
        {
            var res = await _apiHelper.NonBodyQueryAsync<List<Exam>>(HttpMethod.Get, $"/api/Categories/Exams/{categoryId}");
            return res;
        }

        public async Task<ResponseBase<CountExamModel>> CountExam()
        {
            var res = await _apiHelper.NonBodyQueryAsync<CountExamModel>(HttpMethod.Get, "/api/Exams/Admin/Count");
            return res;
        }
    }
}
