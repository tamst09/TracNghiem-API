using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.Result
{
    public class AddListResultRequest
    {
        public AddListResultRequest()
        {
            ResultRequests = new List<AddResultRequest>();
        }

        public List<AddResultRequest> ResultRequests { get; set; }
    }
}
