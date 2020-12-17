using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.FacebookAuth
{
    public class Root
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public AuthResponse authResponse { get; set; }
        public string status { get; set; }
    }
    public class AuthResponse
    {
        public string accessToken { get; set; }
        public string userID { get; set; }
        public int expiresIn { get; set; }
        public string signedRequest { get; set; }
        public string graphDomain { get; set; }
        public int data_access_expiration_time { get; set; }
    }
}
