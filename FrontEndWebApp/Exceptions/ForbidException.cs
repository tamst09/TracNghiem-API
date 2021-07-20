using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndWebApp.Exceptions
{
    public class ForbidException : Exception
    {
        public ForbidException(string message = "You are not permitted to access this resource.") : base(message)
        {
        }
    }
}
