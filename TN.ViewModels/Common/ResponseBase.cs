using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class ResponseBase<T>
    {
        public string StatusCode { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
}
