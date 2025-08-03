using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Domain.Objects
{
    public class HandleResult<T>(bool success, string message, string? Url = null)

    {
        public bool Success { get; set; } = success;
        public string Message { get; set; } = message;
        public string RedirectUrl { get; set; }
        public Dieslovani<T> Dieslovani { get; set; }
        public Odstavka? Odstavka { get; set; }
        public string EmailResult { get; set; } = "";
        public string Duvod { get; set; } = "";
        public T? Data { get; set; }

        public static HandleResult<T> OK(T data, string message = "") => new(true, message) { Data = data };
        public static HandleResult<T> Error(string message = "") => new(false, message) { Data = default };
        public static HandleResult<T> Partial(T data, string message = "") => new(true, message) { Data = data };

    }

    public class HandleResult : HandleResult<object>

    {
        public HandleResult(bool success, string message = "", string? v = null) : base(success, message) { }
        public static HandleResult OK(string message = "") => new(true, message);
        public static new HandleResult Error(string message = "") => new(false, message);
    }
    

}