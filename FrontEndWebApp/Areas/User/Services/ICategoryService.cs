using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public interface ICategoryService
    {
        Task<ResponseBase<List<Category>>> GetAll();
        Task<ResponseBase<List<Exam>>> GetAllExams(int id, string accessToken);
    }
}
