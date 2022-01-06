using Microsoft.AspNetCore.Builder;

namespace HRMS.Utilities.General;

public static class ExceptionHandlerMiddlewareExtensions
{
    public static void UseExceptionHandlerMiddleware(this IApplicationBuilder application) =>
        application.UseMiddleware<ExceptionHandlerMiddleware>();
}
