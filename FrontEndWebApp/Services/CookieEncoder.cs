using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndWebApp.Services
{
    public static class CookieEncoder
    {
        // =========================== ENCODING & DECODING TOKEN BEFORE SAVING TO COOKIES ===========================
        public static string EncodeToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            // to byte[]
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
            // encoding
            var encoded_token = Base64UrlTextEncoder.Encode(tokenBytes);
            return encoded_token;
        }
        public static string DecodeToken(string encodedToken)
        {
            if (string.IsNullOrEmpty(encodedToken))
            {
                return null;
            }
            // get byte[]
            var tokenBytes = Base64UrlTextEncoder.Decode(encodedToken);
            // to string
            return System.Text.Encoding.UTF8.GetString(tokenBytes);
        }
        //===========================
    }
}
