using HRMS.Data.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRMS.Utilities.General
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, HRMSContext db)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionMessageAsync(context, ex, db);
            }
        }

        private static async Task HandleExceptionMessageAsync(HttpContext context, Exception exception, HRMSContext db)
        {
            db.ChangeTracker.Clear();
            var log = new Log
            {
                UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier),
                Ip = context.Connection.RemoteIpAddress.ToString(),
                Controller = context.Request.RouteValues["controller"].ToString(),
                Action = context.Request.RouteValues["action"].ToString(),
                HttpMethod = context.Request.Method,
                Url = context.Request.GetDisplayUrl(),
                Exception = JsonConvert.SerializeObject(exception),
                InsertedDate = DateTime.Now,
                //Error = true
            };

            if (context.Request.HasFormContentType)
            {
                IFormCollection form = await context.Request.ReadFormAsync();
                log.FormContent = JsonConvert.SerializeObject(form);
            }
            db.Log.Add(log);
            await db.SaveChangesAsync();

            if (context.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var result = JsonConvert.SerializeObject(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorMessage = exception.Message
                });
                await context.Response.WriteAsync(result);
            }
            else
            {
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}
