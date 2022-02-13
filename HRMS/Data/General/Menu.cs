using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Menu
    {
        public Menu()
        {
            SubMenu = new HashSet<SubMenu>();
        }

        public int MenuId { get; set; }
        public string NameSq { get; set; }
        public string NameEn { get; set; }
        public bool HasSubMenu { get; set; }
        public bool Active { get; set; }
        public string Icon { get; set; }
        public string Claim { get; set; }
        public string ClaimType { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int OrdinalNumber { get; set; }
        public string Roles { get; set; }
        public string OpenFor { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<SubMenu> SubMenu { get; set; }
    }
}
