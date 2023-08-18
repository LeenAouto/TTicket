namespace TTicket.Models.ResponseModels
{
    public class Response<T>
    {
        public Response(T response, ErrorCode errorCode, string message = "") { 
            Data = response;
            ErrorCode = errorCode;
            Message = message;
        }

        public T Data { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; } 
    }
}
