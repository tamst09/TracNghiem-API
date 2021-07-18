using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class ResponseBase
    {
        public ResponseBase(bool success = true, string msg = "Success.")
        {
            this.success = success;
            this.msg = msg;
        }
        public bool success { get; set; }
        public string msg { get; set; }
    }
    public class ResponseBase<T>
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public T data { get; set; }

        public ResponseBase(T data, bool success=true, string msg="Success.")
        {
            this.success = success;
            this.msg = msg;
            this.data = data;
        }

        public ResponseBase()
        {
        }
    }
}
