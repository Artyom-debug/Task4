using Microsoft.AspNetCore.Identity;

namespace Task4.Models;

public sealed class Result
{
    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }

    public Result(bool success, string[] errors)
    {
        Succeeded = success; 
        Errors = errors;
    }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(string[] errorMessages)
    {
        return new Result(false, errorMessages);
    }

    public static Result FromIdentity(IdentityResult result)
    {
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
    }
}
