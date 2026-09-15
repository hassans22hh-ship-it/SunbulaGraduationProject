using SharedKernel;

namespace Sunbula.Middleware
{
    public class GlobalExceptionMiddelware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionMiddelware(RequestDelegate next,ILogger<GlobalExceptionHandler>logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"appear wrong:{ex.Message}");
                await HandleExceptions(context, ex);
            }
        }

        private async  Task HandleExceptions(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var result= new ErrorDetails()
            {
                StatusCode=context.Response.StatusCode,
                ErrorMessage=ex.Message
            }.ToString();

         await   context.Response.WriteAsync(result);

        }
    }
}
