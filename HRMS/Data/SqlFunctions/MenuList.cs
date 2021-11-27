namespace HRMS.Data.SqlFunctions
{
    public class MenuList
    {
        public int MenuId { get; set; }
        public string MenuTitle { get; set; }
        public string MenuController { get; set; }
        public string MenuAction { get; set; }
        public string MenuOpenFor { get; set; }
        public int? SubMenuId { get; set; }
        public string SubMenuTitle { get; set; }
        public string SubMenuController { get; set; }
        public string SubMenuAction { get; set; }
        public string SubMenuOpenFor { get; set; }
        public string MenuIcon { get; set; }
        public string SubMenuIcon { get; set; }
        public bool HasSubMenu { get; set; }
        public int MenuOrdinalNumber { get; set; }
        public int SubMenuOrdinalNumber { get; set; }
    }
}
