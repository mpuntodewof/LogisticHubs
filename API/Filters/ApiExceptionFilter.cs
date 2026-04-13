using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
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
            }
        }
    }
}
