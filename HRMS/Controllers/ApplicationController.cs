using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models.Application;
using HRMS.Repository;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class ApplicationController : BaseController
{
    private readonly IFunctionRepository function;

    public ApplicationController(IFunctionRepository function,
        HRMS_WorkContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.function = function;
    }

    [Authorize(Policy = "60:r"), Description("Arb Tahiri", "Entry form.")]
    public IActionResult Index() => View();

    #region Application logs

    [HttpGet, Authorize(Policy = "61:r"), Description("Arb Tahiri", "Entry form for application logs.")]
    public IActionResult Logs() => View();

    [HttpPost, Authorize(Policy = "61:r"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to search for logs.")]
    public async Task<IActionResult> SearchLogs(LogSearch search)
    {
        string[] dates = search.Date.Split(" - ");
        DateTime startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);
        DateTime endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy H:mm", CultureInfo.InvariantCulture);

        var list = (await function.Logs(search.Role, search.User, startDate, endDate, search.Ip, search.Controller, search.Action, search.HttpMethod, search.Error))
            .Select(a => new LogDetails
            {
                LogIde = CryptoSecurity.Encrypt(a.LogId),
                Ip = a.Ip,
                Controller = a.Controller,
                Action = a.Action,
                Description = a.Description,
                Exception = a.Exception,
                FormContent = a.FormContent,
                HttpMethod = a.HttpMethod,
                Username = a.Username,
                InsertDate = a.InsertDate
            }).ToList();
        return Json(list.OrderByDescending(a => a.InsertDate));
    }

    #endregion

    #region Security logs

    [Authorize(Policy = "62:r"), Description("Arb Tahiri", "Entry form for security logs.")]
    public IActionResult Security() => View();

    [SupportedOSPlatform("windows")]
    [HttpPost, Authorize(Policy = "62:r"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to search for security logs.")]
    public IActionResult SearchSecurity(ServerSearch search)
    {
        string[] dates = search.LogTime.Split(" - ");
        DateTime startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);
        DateTime endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy H:mm", CultureInfo.InvariantCulture);

        var eventLogs = EventLog.GetEventLogs();
        var logs = new List<ServerLogs>();

        foreach (var log in eventLogs)
        {
            foreach (EventLogEntry entry in log.Entries)
            {
                if (entry.TimeGenerated >= startDate && entry.TimeGenerated <= endDate)
                {
                    if (search.EventLogEntryType != null)
                    {
                        if (entry.EntryType == search.EventLogEntryType)
                        {
                            logs.Add(new ServerLogs
                            {
                                EntryType = entry.EntryType.ToString(),
                                EventLogEntryType = entry.EntryType,
                                Machine = entry.MachineName,
                                Message = entry.Message,
                                Source = entry.Source,
                                Time = entry.TimeGenerated,
                                Username = entry.UserName
                            });
                        }
                    }
                    else
                    {
                        logs.Add(new ServerLogs
                        {
                            EntryType = entry.EntryType.ToString(),
                            EventLogEntryType = entry.EntryType,
                            Machine = entry.MachineName,
                            Message = entry.Message,
                            Source = entry.Source,
                            Time = entry.TimeGenerated,
                            Username = entry.UserName
                        });
                    }
                }
            }
        }
        return PartialView(logs);
    }

    #endregion
}
