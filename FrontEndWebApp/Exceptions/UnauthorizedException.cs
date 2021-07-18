using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndWebApp.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "This feature need to log in.") : base(message)
        {
        }
    }
}
