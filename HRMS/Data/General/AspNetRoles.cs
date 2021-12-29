﻿using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class AspNetRoles
    {
        public AspNetRoles()
        {
            AspNetRoleClaims = new HashSet<AspNetRoleClaims>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
            User = new HashSet<AspNetUsers>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string NameSq { get; set; }
        public string NameEn { get; set; }
        public string DescriptionSq { get; set; }
        public string DescriptionEn { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual ICollection<AspNetRoleClaims> AspNetRoleClaims { get; set; }

        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }

        public virtual ICollection<AspNetUsers> User { get; set; }
    }
}
