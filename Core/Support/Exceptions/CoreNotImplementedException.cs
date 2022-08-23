using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Support.Exceptions
{
    public class CoreNotImplementedException : NotImplementedException
    {
        public CoreNotImplementedException(string message)
            : base(message)
        {

        }
    }
}
