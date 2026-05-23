namespace backend.Application.Common.Models;

public sealed class Result<T>
{
    private Result(bool succeeded, T? data, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Data = data;
        Errors = errors.ToArray();
    }

    public bool Succeeded { get; }
    public T? Data { get; }
    public string[] Errors { get; }

    public static Result<T> Success(T data) => new(true, data, Array.Empty<string>());

    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
}

