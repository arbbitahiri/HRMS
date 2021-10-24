using Microsoft.AspNetCore.Builder;

namespace HRMS.Utilities.General
{
    public static class ExceptionHandlerExtensions
    {
        public static void UseExceptionHandler(this IApplicationBuilder application) =>
            application.UseMiddleware<ExceptionHandler>();
    }
}
