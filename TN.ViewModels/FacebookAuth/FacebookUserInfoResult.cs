using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.FacebookAuth
{
    public class FacebookUserInfoResult
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
