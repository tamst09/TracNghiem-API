using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Services
{
    public interface IApiHelper
    {
        /// <summary>
        /// Send no body -> Receive body
        /// </summary>
        Task<ResponseBase<TResponse>> NonBodyQueryAsync<TResponse>(HttpMethod httpMethod, string url) where TResponse : class;
        /// <summary>
        /// Send body -> Receive body
        /// </summary>
        Task<ResponseBase<TResponse>> QueryAsync<TRequest, TResponse>(HttpMethod httpMethod, string url, TRequest request) where TRequest : class where TResponse : class;
        /// <summary>
        /// Send body -> Receive no body
        /// </summary>
        Task<ResponseBase> CommandAsync<TRequest>(HttpMethod httpMethod, string url, TRequest request) where TRequest : class;
        /// <summary>
        /// Send no body -> Receive no body
        /// </summary>
        Task<ResponseBase> NonBodyCommandAsync(HttpMethod httpMethod, string url);
    }
}
