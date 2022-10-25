using System.Net;
using System.Runtime.Serialization;

namespace Core.Common.Exceptions
{
    [Serializable]
    public sealed class CoreException : Exception
    {
        private readonly HttpStatusCode statusCode;

        public CoreException(string message, HttpStatusCode status)
            : base(message)
        {
            statusCode = status;
        }

        private CoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.statusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode))!;
        }

        public HttpStatusCode StatusCode
        {
            get { return this.statusCode; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("StatusCode", this.StatusCode, typeof(HttpStatusCode));

            base.GetObjectData(info, context);
        }
    }
}
