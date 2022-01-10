using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class RealRole
    {
        public int RealRoleId { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetRoles Role { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
