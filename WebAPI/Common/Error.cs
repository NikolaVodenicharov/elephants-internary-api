namespace WebAPI.Common
{
    public class Error
    {
        public string Message { get; set; }

        public Error()
        {
            Message = string.Empty;
        }

        public Error(string message)
        {
            Message = message;
        }
    }
}
