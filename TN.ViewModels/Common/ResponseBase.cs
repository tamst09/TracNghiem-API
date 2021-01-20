using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class ResponseBase<T>
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
}
