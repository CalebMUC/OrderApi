using System.Reflection;
using Minimart_Api.DTOS;

namespace Minimart_Api.TempModels
{
    public class SubModules
    {
        public int SubModuleID { get; set; } // Primary Key
        public string SubModuleName { get; set; }
        public int ModuleID { get; set; } // Foreign Key to Modules
        public string SubModuleUrl { get; set; }
        public int Order { get; set; } = 0;

        // Navigation Property for Module
        public Modules Module { get; set; }

        // Navigation Property for RolePermissions
        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();

        public ICollection<SubModuleCategories> SubModuleCategories { get; set; } = new List<SubModuleCategories>();

    }
}
