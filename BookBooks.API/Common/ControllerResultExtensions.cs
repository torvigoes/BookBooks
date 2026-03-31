using BookBooks.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.API.Common;

public static class ControllerResultExtensions
{
    public static IActionResult ToFailureActionResult(this ControllerBase controller, string? error)
    {
        var message = string.IsNullOrWhiteSpace(error)
            ? "An unexpected error occurred."
            : error;

        var statusCode = ResolveStatusCode(message);
        return controller.StatusCode(statusCode, new { Error = message });
    }

    public static IActionResult ToActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        Func<T, IActionResult> onSuccess)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return onSuccess(result.Value);
        }

        return controller.ToFailureActionResult(result.Error);
    }

    public static IActionResult ToActionResult(
        this ControllerBase controller,
        Result result,
        Func<IActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess();
        }

        return controller.ToFailureActionResult(result.Error);
    }

    private static int ResolveStatusCode(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status404NotFound;
        }

        if (error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("invalid email or password", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("claim not found", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status401Unauthorized;
        }

        if (error.Contains("forbidden", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("only update your own", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("only delete your own", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status403Forbidden;
        }

        return StatusCodes.Status400BadRequest;
    }
}
