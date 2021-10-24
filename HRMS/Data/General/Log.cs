using System;
using System.Collections.Generic;

#nullable disable

namespace HRMS.Data.General
{
    public partial class Log
    {
        public long LogId { get; set; }
        public string UserId { get; set; }
        public string Ip { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string HttpMethod { get; set; }
        public string Url { get; set; }
        public string FormContent { get; set; }
        public string Exception { get; set; }
        public DateTime InsertedDate { get; set; }
        public bool Error { get; set; }
    }
}
