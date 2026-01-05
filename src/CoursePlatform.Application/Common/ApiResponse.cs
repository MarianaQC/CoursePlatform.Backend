namespace CoursePlatform.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string message = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = new List<string>()
        };
    }

    public static ApiResponse<T> Fail(string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = error,
            Data = default,
            Errors = new List<string> { error }
        };
    }

    public static ApiResponse<T> Fail(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Error de validación",
            Data = default,
            Errors = errors
        };
    }
}