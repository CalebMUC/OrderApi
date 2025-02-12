using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories.SystemSecurityRepo
{
    public class SystemSecurityRepo : ISystemSecurityRepo
    {
        private readonly MinimartDBContext _dbContext;
        public SystemSecurityRepo(MinimartDBContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<List<ModuleDto>> GetRoleModules(string RoleID)
        {
            try
            {
                var modules = await _dbContext.RolePermissions
                    .Where(rp => rp.RoleID == RoleID)
                    .Select(rp => new ModuleDto
                    {
                        ModuleId = rp.ModuleID,
                        ModuleName = rp.ModuleName,
                        SubModules = rp.Module.Submodules
                            .Where(sm => sm.SubModuleID == rp.SubModuleID)
                            .Select(sm => new SubModuleDto
                            {
                                SubModuleId = sm.SubModuleID,
                                SubModuleName = sm.SubModuleName,
                                SubModuleUrl = sm.SubModuleUrl,
                                Order = sm.Order
                            })
                            .ToList()
                    })
                    .Distinct() // Ensure unique modules
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
