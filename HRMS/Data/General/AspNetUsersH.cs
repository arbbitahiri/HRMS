using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class AspNetUsersH
    {
        public int HistoryAspNetUsersId { get; set; }
        public string Id { get; set; }
        public string PersonalNumber { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NormalizedUserName { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public bool AllowNotification { get; set; }
        public int Language { get; set; }
        public int Mode { get; set; }
        public string ProfileImage { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string Reason { get; set; }
        public DateTime InsertedDate { get; set; }
        public string InsertedFrom { get; set; }
    }
}
