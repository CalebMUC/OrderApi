namespace Minimart_Api.TempModels
{
    public class Roles
    {
        public string RoleID { get; set; } // Primary Key
        public string RoleName { get; set; }
        public int AccessLevel { get; set; }

        // Navigation Property for RolePermissions
        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();
    }
}
