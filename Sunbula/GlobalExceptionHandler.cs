using DebtDomain.Exceptions;
using Domain.Exceptions;
using FinanceDomain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Sunbula
{
    /// <summary>
    /// Global exception handler using IExceptionHandler (ASP.NET Core 8+).
    /// Converts known domain exceptions to appropriate HTTP status codes.
    /// </summary>
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                UserNotFoundException => (HttpStatusCode.NotFound, "User Not Found"),
                UnauthorizedException => (HttpStatusCode.Unauthorized, "Authentication Failed"),
                ValidationException => (HttpStatusCode.BadRequest, "Validation Error"),

                // Domain not-found exceptions
                DebtNotFoundException => (HttpStatusCode.NotFound, "Debt not found"),
                WalletNotFoundException => (HttpStatusCode.NotFound, "Wallet not found"),
                FinancialCategoryNotFoundException => (HttpStatusCode.NotFound, "Financial category not found"),
                FinancialTransactionNotFoundException => (HttpStatusCode.NotFound, "Financial transaction not found"),

                // Domain business rule violations
                DebtAlreadyPaidException => (HttpStatusCode.Conflict, "Debt is already paid"),
                PaymentExceedsRemainingAmountException => (HttpStatusCode.BadRequest, "Payment exceeds remaining amount"),
                InvalidPaymentDateException => (HttpStatusCode.BadRequest, "Invalid payment date"),
                InsufficientBalanceException => (HttpStatusCode.BadRequest, "Insufficient balance"),

                // Auth exceptions
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),

                // Validation
                ArgumentException => (HttpStatusCode.BadRequest, "Validation error"),
                InvalidOperationException => (HttpStatusCode.Conflict, "Invalid operation"),

                // Fallback
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = (int)statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
