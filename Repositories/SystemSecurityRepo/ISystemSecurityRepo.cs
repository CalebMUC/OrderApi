using Minimart_Api.DTOS;

namespace Minimart_Api.Repositories.SystemSecurityRepo
{
    public interface ISystemSecurityRepo
    {
        Task <List<ModuleDto>> GetRoleModules(string RoleID);

        Task<List<SubModuleCategoriesDto>> GetSubModuleCategories(int subModuleID);
    }
}
