namespace BlazorApp.Client.Models
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        public bool IsConflict => StatusCode == 409;
        public bool IsNotFound => StatusCode == 404;
        public bool IsForbidden => StatusCode == 403;
        public bool IsUnauthorized => StatusCode == 401;
        public bool IsServerError => StatusCode >= 500;

        public static ApiResult<T> Ok(T data) => new()
        {
            Success = true,
            Data = data,
            StatusCode = 200
        };

        public static ApiResult<T> Fail(int statusCode, string? errorMessage = null) => new()
        {
            Success = false,
            StatusCode = statusCode,
            ErrorMessage = errorMessage
        };
    }

    public class ApiResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        public bool IsConflict => StatusCode == 409;
        public bool IsNotFound => StatusCode == 404;
        public bool IsServerError => StatusCode >= 500;

        public static ApiResult Ok() => new() { Success = true, StatusCode = 200 };

        public static ApiResult Fail(int statusCode, string? errorMessage = null) => new()
        {
            Success = false,
            StatusCode = statusCode,
            ErrorMessage = errorMessage
        };
    }
}
