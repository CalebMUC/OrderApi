using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Security;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.SystemSecurityRepo
{
    public class SystemSecurityRepo : ISystemSecurityRepo
    {
        private readonly MinimartDBContext _dbContext;
        public SystemSecurityRepo(MinimartDBContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<List<SubModuleCategoriesDto>> GetSubModuleCategories(int subModuleID)
        {
            try
            {
                var subModuleCategories = await _dbContext.SubModuleCategories
                                .Where(sm => sm.SubModuleID == subModuleID)
                                .OrderBy(sc => sc.Order)
                                .Select(sc => new SubModuleCategoriesDto
                                {
                                    SubCategoryID = sc.SubCategoryID,
                                    SubCategoryName = sc.SubCategoryName,
                                    SubCategoryUrl = sc.SubCategoryUrl,
                                    Order = sc.Order,
                                    ModuleName = sc.Submodule.SubModuleName
                                }).ToListAsync();

                return subModuleCategories;
            }
            catch (Exception ex) {
                return [];
                Console.WriteLine($"Sql Error is {ex.Message}");
            }
        }

        public async Task<List<ModuleDto>> GetRoleModules(string RoleID)
        {
            try
            {
                var modules = await _dbContext.RolePermissions
                    .Where(rp => rp.RoleID == RoleID)
                    .GroupBy(rp => new { rp.ModuleID, rp.ModuleName })
                    .Select(g => new ModuleDto
                    {
                        ModuleID = g.Key.ModuleID,
                        ModuleName = g.Key.ModuleName,
                        SubModules = g.Select(
                            rp => new SubModuleDto {
                                SubModuleID = rp.SubModuleID,
                                SubModuleName = rp.SubModuleName,
                                SubModuleUrl = rp.Submodule.SubModuleUrl,
                                Order = rp.Submodule.Order
                            }
                            ).ToList()
                    })
                   // .Distinct() // Ensure unique modules
                    .ToListAsync();

                
                return modules;
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error fetching modules: {ex.Message}");
                return []; // Return an empty list in case of error
            }
        }
    }
}
