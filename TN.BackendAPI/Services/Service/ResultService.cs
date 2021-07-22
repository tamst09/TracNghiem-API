using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Result;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.Service
{
    public class ResultService : IResultService
    {
        private readonly TNDbContext _db;
        public ResultService(TNDbContext db)
        {
            _db = db;
        }

        public async Task<ResponseBase> AddListResult(AddListResultRequest request, int userId)
        {
            var results = _db.Results.Where(r => r.UserID == userId).ToList();
            foreach (var item in request.ResultRequests)
            {
                var t = results.FirstOrDefault(r => r.QuestionID == item.QuestionId);
                if(t != null)
                {
                    t.OptChoose = item.Choice;
                }
                else
                {
                    _db.Results.Add(new Result()
                    {
                        QuestionID = item.QuestionId,
                        UserID = userId,
                        OptChoose = item.Choice
                    });
                }
            }
            try
            {
                _db.SaveChanges();
                return new ResponseBase();
            }
            catch (Exception e)
            {
                return new ResponseBase(success: false, msg: e.Message);
            }
        }

        public async Task<ResponseBase> AddResult(AddResultRequest request, int userId)
        {
            var existed = _db.Results.FirstOrDefault(r => r.UserID == userId && r.QuestionID == request.QuestionId);
            if (existed == null)
            {
                _db.Results.Add(new Result()
                {
                    UserID = userId,
                    QuestionID = request.QuestionId,
                    OptChoose = request.Choice
                });
            }
            else
            {
                existed.OptChoose = request.Choice;
            }
            try
            {
                _db.SaveChanges();
                return new ResponseBase();
            }
            catch (Exception e)
            {
                return new ResponseBase(success: false, msg: e.Message);
            }
        }
    }
}
