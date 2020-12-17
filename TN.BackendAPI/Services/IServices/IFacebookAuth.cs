using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.FacebookAuth;

namespace TN.BackendAPI.Services.IServices
{
    public interface IFacebookAuth
    {
        Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accesstoken);
        Task<FacebookUserInfoResult> GetUserInfoAsync(string accesstoken);
    }
}
