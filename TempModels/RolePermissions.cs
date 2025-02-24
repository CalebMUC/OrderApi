using System.Data;
using System.Reflection;

namespace Minimart_Api.TempModels
{
    public class RolePermissions
    {
        public int RolePermissionID { get; set; } // Primary Key

        // Foreign Key to Roles
        public string RoleID { get; set; }
        public string RoleName { get; set; }

        // Foreign Key to Modules
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }

        // Foreign Key to Submodules
        public int SubModuleID { get; set; }
        public string SubModuleName { get; set; }

        // Navigation Properties
        public Roles Role { get; set; }
        public Modules Module { get; set; }
        public SubModules Submodule { get; set; }
    }
}
