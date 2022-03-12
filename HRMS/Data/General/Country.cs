using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Country
    {
        public Country()
        {
            Staff = new HashSet<Staff>();
        }

        public int CountryId { get; set; }
        public string NameSq { get; set; }
        public string NameEn { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual ICollection<Staff> Staff { get; set; }
    }
}
