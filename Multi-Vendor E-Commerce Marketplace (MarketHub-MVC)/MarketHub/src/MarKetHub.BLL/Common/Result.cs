namespace MarketHub.BLL.Common;

/// <summary>
/// Expected business failures (not enough stock, not found...) are returned as a Result.
/// Exceptions are reserved for unexpected errors.
/// </summary>
public class Result
{
    public bool Success { get; protected init; }
    public string Message { get; protected init; } = string.Empty;

    /// <summary>True when the failure means "this thing does not exist (for you)" -> controllers answer 404.</summary>
    public bool NotFound { get; protected init; }

    public static Result Ok(string message = "") => new() { Success = true, Message = message };
    public static Result Fail(string message) => new() { Success = false, Message = message };
    public static Result NotFoundResult(string message = "Not found.") =>
        new() { Success = false, NotFound = true, Message = message };
}

public class Result<T> : Result
{
    public T? Value { get; private init; }

    public static Result<T> Ok(T value, string message = "") =>
        new() { Success = true, Value = value, Message = message };

    public new static Result<T> Fail(string message) =>
        new() { Success = false, Message = message };

    public new static Result<T> NotFoundResult(string message = "Not found.") =>
        new() { Success = false, NotFound = true, Message = message };
}
