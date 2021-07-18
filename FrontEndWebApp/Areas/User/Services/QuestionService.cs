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

namespace FrontEndWebApp.Areas.User.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IApiHelper _apiHelper;

        public QuestionService(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase> Create(QuestionModel model)
        {
            var response = await _apiHelper.CommandAsync(HttpMethod.Post, "/api/Questions", model);
            return response;
        }

        public async Task<ResponseBase> DeleteMany(DeleteManyModel<int> model)
        {
            var response = await _apiHelper.CommandAsync(HttpMethod.Post, "/api/Questions/DeleteMany", model);
            return response;
        }

        public async Task<ResponseBase<List<Question>>> GetByExamID(int examID)
        {
            GetQuestionsByExamRequest request = new GetQuestionsByExamRequest(examID);
            var response = await _apiHelper.QueryAsync<GetQuestionsByExamRequest, List<Question>>(HttpMethod.Post, "/api/Questions/GetByExam", request);
            return response;
        }

        public async Task<ResponseBase<Question>> GetByID(int id)
        {
            var response = await _apiHelper.NonBodyQueryAsync<Question>(HttpMethod.Get, $"/api/Question/{id}");
            return response;
        }

        public async Task<ResponseBase<PagedResult<Question>>> GetPagedQuestion(QuestionPagingRequest model)
        {
            var response = await _apiHelper.QueryAsync<QuestionPagingRequest, PagedResult<Question>>(HttpMethod.Post, $"/api/Question/Paged", model);
            return response;
        }

        public async Task<ResponseBase> Update(QuestionModel model)
        {
            var response = await _apiHelper.CommandAsync(HttpMethod.Put, $"/api/Question", model);
            return response;
        }
    }
}
