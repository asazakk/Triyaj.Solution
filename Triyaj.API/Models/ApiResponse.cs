namespace Triyaj.API.Models;

/// <summary>
/// API yanıtları için genel response modeli
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }

    public static ApiResponse Ok(object? data = null, string? message = null) 
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse Error(string message) 
        => new() { Success = false, Message = message };
}

/// <summary>
/// API yanıtları için generic response modeli
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null) 
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Error(string message) 
        => new() { Success = false, Message = message };
}

