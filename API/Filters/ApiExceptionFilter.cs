using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;
        private readonly IHostEnvironment _environment;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case DbUpdateConcurrencyException ex:
                    _logger.LogWarning(ex, "Concurrency conflict detected.");
                    context.Result = new ConflictObjectResult(new
                    {
                        error = "This record was modified by another user. Please reload and try again."
                    });
                    context.ExceptionHandled = true;
                    break;

                case ConcurrencyConflictException ex:
                    _logger.LogWarning(ex, "Concurrency conflict: {Message}", ex.Message);
                    context.Result = new ConflictObjectResult(new
                    {
                        error = ex.Message
                    });
                    context.ExceptionHandled = true;
                    break;

                case UnauthorizedAccessException ex:
                    context.Result = new UnauthorizedObjectResult(new
                    {
                        error = ex.Message
                    });
                    context.ExceptionHandled = true;
                    break;

                case InvalidOperationException ex:
                    _logger.LogWarning(ex, "Business rule violation: {Message}", ex.Message);
                    context.Result = new ConflictObjectResult(new
                    {
                        error = ex.Message
                    });
                    context.ExceptionHandled = true;
                    break;

                case KeyNotFoundException ex:
                    context.Result = new NotFoundObjectResult(new
                    {
                        error = ex.Message
                    });
                    context.ExceptionHandled = true;
                    break;

                default:
                    _logger.LogError(context.Exception, "Unhandled exception on {Method} {Path}",
                        context.HttpContext.Request.Method,
                        context.HttpContext.Request.Path);

                    var errorResponse = _environment.IsDevelopment()
                        ? new { error = context.Exception.Message, detail = context.Exception.StackTrace }
                        : new { error = "An unexpected error occurred. Please try again later.", detail = (string?)null };

                    context.Result = new ObjectResult(errorResponse) { StatusCode = 500 };
                    context.ExceptionHandled = true;
                    break;
            }
        }
    }
}
