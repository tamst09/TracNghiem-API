using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.FavoriteExam;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public class FavoriteExamService : IFavoriteExamService
    {
        private readonly IApiHelper _apiHelper;

        public FavoriteExamService(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase> Add(AddFavoriteExamRequest addFavoriteExamRequest)
        {
            return await _apiHelper.CommandAsync(HttpMethod.Post, "/api/FavoriteExam", addFavoriteExamRequest);
        }

        public async Task<ResponseBase> Delete(DeleteFavoriteExamRequest deleteFavoriteExamRequest)
        {
            return await _apiHelper.CommandAsync(HttpMethod.Post, "/api/FavoriteExam/Remove", deleteFavoriteExamRequest);
        }

        public async Task<ResponseBase<List<Exam>>> GetExams(GetAllFavoriteRequest getAllFavoriteRequest)
        {
            return await _apiHelper.NonBodyQueryAsync<List<Exam>>(HttpMethod.Get, "/api/FavoriteExam");
        }
    }
}
