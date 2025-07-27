using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Domain.Objects
{
    public class HandleResult<T>

    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Dieslovani<T> Dieslovani { get; set; }
        public Odstavka? Odstavka { get; set; }
        public string EmailResult { get; set; } = "";
        public string Duvod { get; set; } = "";
        public T? Data { get; set; }

        public HandleResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static HandleResult<T> OK(T data, string message = "") => new(true, message) { Data = data };
        public static HandleResult<T> Error(string message = "") => new(false, message) { Data = default };
        public static HandleResult<T> Partial(T data, string message = "") => new(true, message) { Data = data };

    }

    public class HandleResult : HandleResult<object>

    {
        public HandleResult(bool success, string message = "") : base(success, message) { }
        public static new HandleResult OK(string message = "") => new(true, message);
        public static new HandleResult Error(string message = "") => new(false, message);
    }
    
    public static class HandleResultExtensions
    {
        public static HandleResult<T> WithData<T>(this HandleResult<T> result, T data)
        {
            result.Data = data;
            return result;
        }

        public static HandleResult WithMessage(this HandleResult result, string message)
        {
            result.Message = message;
            return result;
        }
    }

}