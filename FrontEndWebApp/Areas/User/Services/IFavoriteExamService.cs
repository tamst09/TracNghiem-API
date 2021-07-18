using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.FavoriteExam;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public interface IFavoriteExamService
    {
        Task<ResponseBase<List<Exam>>> GetExams(GetAllFavoriteRequest getAllFavoriteRequest);
        Task<ResponseBase> Add(AddFavoriteExamRequest addFavoriteExamRequest);
        Task<ResponseBase> Delete(DeleteFavoriteExamRequest deleteFavoriteExamRequest);
    }
}
