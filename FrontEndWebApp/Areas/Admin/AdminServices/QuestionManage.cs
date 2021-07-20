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
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class QuestionManage : IQuestionManage
    {
        private readonly IApiHelper _apiHelper;
        public QuestionManage(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase> Create(QuestionModel model)
        {
            var res = await _apiHelper.CommandAsync<QuestionModel>(HttpMethod.Post, "/api/Questions", model);
            return res;
        }

        public async Task<ResponseBase> Delete(int id)
        {
            var res = await _apiHelper.NonBodyCommandAsync(HttpMethod.Delete, $"/api/Questions/{id}");
            return res;
        }

        public async Task<ResponseBase> DeleteMany(DeleteManyModel<int> lstId)
        {
            var res = await _apiHelper.CommandAsync<DeleteManyModel<int>>(HttpMethod.Post, $"/api/Questions/DeleteMany", lstId);
            return res;
        }

        public async Task<ResponseBase<List<Question>>> GetAll()
        {
            var res = await _apiHelper.NonBodyQueryAsync<List<Question>>(HttpMethod.Get, $"/api/Questions");
            return res;
        }

        public async Task<ResponseBase<List<Question>>> GetAllByExamID(int examID)
        {
            var request = new GetQuestionsByExamRequest(examID);
            var res = await _apiHelper.QueryAsync<GetQuestionsByExamRequest, List<Question>>(HttpMethod.Post, $"/api/Questions/GetByExam", request);
            return res;
        }

        public async Task<ResponseBase<PagedResult<Question>>> GetAllPaging(QuestionPagingRequest model)
        {
            var res = await _apiHelper.QueryAsync<QuestionPagingRequest,PagedResult<Question>>(HttpMethod.Post, $"/api/Questions/Paged", model);
            return res;
        }

        public async Task<ResponseBase<Question>> GetByID(int id)
        {
            var res = await _apiHelper.NonBodyQueryAsync<Question>(HttpMethod.Post, $"/api/Questions/{id}");
            return res;
        }

        public async Task<ResponseBase<CountQuestionModel>> CountQuestion()
        {
            var res = await _apiHelper.NonBodyQueryAsync<CountQuestionModel>(HttpMethod.Get, "/api/Questions/Count");
            return res;
        }

        public async Task<ResponseBase> Update(QuestionModel model)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Put, "/api/Questions", model);
            return res;
        }
    }
}
