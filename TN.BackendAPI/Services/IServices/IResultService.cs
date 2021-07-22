using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Result;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface IResultService
    {
        Task<ResponseBase> AddResult(AddResultRequest request, int userId);
        Task<ResponseBase> AddListResult(AddListResultRequest request, int userId);
    }
}
