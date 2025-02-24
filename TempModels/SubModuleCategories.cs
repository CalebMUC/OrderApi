namespace Minimart_Api.TempModels
{
    public class SubModuleCategories
    {
        public int SubCategoryID { get; set; }
        public string SubCategoryName { get; set; }
        public int SubModuleID { get; set; }
        public string SubCategoryUrl { get; set; }
        public int Order { get; set; }
        public SubModules Submodule { get; set; }
    }
}
