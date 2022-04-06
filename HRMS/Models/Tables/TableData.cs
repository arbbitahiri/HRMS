using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Models.Tables;

public class TableData
{
    public LookUpTable Table { get; set; }
    public string Title { get; set; }
    public List<DataList> DataList { get; set; }
}

public class DataList
{
    public string Ide { get; set; }
    public string NameSQ { get; set; }
    public string NameEN { get; set; }
    public string DescriptionSQ { get; set; }
    public string DescriptionEN { get; set; }
    public string OtherData { get; set; }
    public bool Active { get; set; }
}
