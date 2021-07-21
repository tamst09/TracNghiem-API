using FrontEndWebApp.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

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

        public async Task<ResponseBase> AddListQuestions(AddListQuestionRequest model)
        {
            var response = await _apiHelper.CommandAsync(HttpMethod.Post, "/api/Questions/AddListQuestions", model);
            return response;
        }

        public async Task<ResponseBase> DeleteMany(DeleteManyModel<int> model)
        {
            var response = await _apiHelper.CommandAsync(HttpMethod.Post, "/api/Questions/DeleteMany", model);
            return response;
        }

        public async Task<ResponseBase> Delete(int questionId)
        {
            var response = await _apiHelper.NonBodyCommandAsync(HttpMethod.Delete, $"/api/Questions/{questionId}");
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
            var response = await _apiHelper.NonBodyQueryAsync<Question>(HttpMethod.Get, $"/api/Questions/{id}");
            return response;
        }

        public async Task<ResponseBase<PagedResult<Question>>> GetPagedQuestion(QuestionPagingRequest model)
        {
            var response = await _apiHelper.QueryAsync<QuestionPagingRequest, PagedResult<Question>>(HttpMethod.Post, $"/api/Questions/Paged", model);
            return response;
        }

        public async Task<ResponseBase> Update(QuestionModel model)
        {
            var response = await _apiHelper.CommandAsync(HttpMethod.Put, $"/api/Questions", model);
            return response;
        }
    }
}
