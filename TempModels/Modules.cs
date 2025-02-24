namespace Minimart_Api.TempModels
{
    public class Modules
    {
        public int ModuleID { get; set; } // Primary Key
        public string ModuleName { get; set; }
        public string CreatedBy { get; set; }
        public string MenuUrl { get; set; }
        public DateTime CreatedOn { get; set; }

        // Navigation Property for Submodules
        public ICollection<SubModules> Submodules { get; set; } = new List<SubModules>();

        // Navigation Property for RolePermissions
        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();
    }
}
