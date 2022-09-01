using System.Net;

namespace Core.Common.Exceptions
{
    public class CoreException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public CoreException(string message, HttpStatusCode status)
            : base(message)
        {
            StatusCode = status;
        }  
    }
}
