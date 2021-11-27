namespace HRMS.Models.Menu;
public class MenuDetails
{
    public int RowNo { get; set; }
    public string MenuIde { get; set; }
    public string Title { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public bool HasSubMenu { get; set; }
    public string Icon { get; set; }
}
