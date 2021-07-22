using FrontEndWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Result;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public class ResultService : IResultService
    {
        private readonly IApiHelper _apiHelper;

        public ResultService(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase> AddListResult(AddListResultRequest request)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Post, "/api/Results/AddList", request);
            return res;
        }
    }
}
