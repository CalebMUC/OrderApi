using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minimart_Api.DTOS;
using Minimart_Api.Services.SignalR;
using Minimart_Api.TempModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minimart_Api.Repositories.SystemMerchantsRepository
{
    public class SystemMerchantRepo : ISystemMerchantRepo
    {
        private readonly MinimartDBContext _dbContext;
        private readonly ILogger<SystemMerchantRepo> _logger;
        private readonly IHubContext<ActivityHub> _hubContext;
        public SystemMerchantRepo(MinimartDBContext dBContext, 
            ILogger<SystemMerchantRepo> logger, 
            IHubContext<ActivityHub> hubContext)
        {
            _hubContext = hubContext;
            _dbContext = dBContext;
            _logger = logger;
        }

        public async Task<IEnumerable<SystemMerchants>> GetAllMerchantsAsync()
        {
            return await _dbContext.SystemMerchants.ToListAsync();
        }

        public async Task<SystemMerchants> GetMerchantByIdAsync(int merchantId)
        {
            return await _dbContext.SystemMerchants.FindAsync(merchantId);
        }

        public async Task<ResponseStatus> AddMerchantsAsync(SystemMerchantsDto systemMerchants)
        {
            try
            {
                if (systemMerchants == null)
                {
                    return new ResponseStatus { ResponseStatusId = 400, ResponseMessage = "Invalid merchant data" };
                }

                bool exists = await _dbContext.SystemMerchants
                    .AnyAsync(m => m.Email == systemMerchants.Email || m.BusinessRegistrationNo == systemMerchants.BusinessRegistrationNo);

                if (exists)
                {
                    return new ResponseStatus { ResponseStatusId = 400, ResponseMessage = "Merchant With the BusinessRegistrationNo and Email  Already Exists" };
                }

                var newMerchant = MapDtoToEntity(systemMerchants);
                _dbContext.SystemMerchants.Add(newMerchant);
                await _dbContext.SaveChangesAsync();

                _hubContext.Clients.All.SendAsync("RecieveNewMerchant", $"New Merchant MerchantName : {newMerchant.MerchantName} BusinessName : {newMerchant.BusinessName}");

                return new ResponseStatus { ResponseStatusId = 200, ResponseMessage = "Merchant Saved Successfully" };
            }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error while adding merchant.");
                    return new ResponseStatus { ResponseStatusId = 500, ResponseMessage = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while adding merchant.");
                    return new ResponseStatus { ResponseStatusId = 500, ResponseMessage = $"Internal server error: {ex.Message}" };
                }
           }

        public async Task<ResponseStatus> UpdateMerchantsAsync(SystemMerchantsDto systemMerchants)
        {
            try
            {
                var existingMerchant = await _dbContext.SystemMerchants.FindAsync(systemMerchants.MerchantID);
                if (existingMerchant == null)
                {
                    return new ResponseStatus { ResponseStatusId = 400, ResponseMessage = "Merchant doesn't exist" };
                }

                UpdateEntityFromDto(existingMerchant, systemMerchants);
                await _dbContext.SaveChangesAsync();

                _hubContext.Clients.All.SendAsync("ReceiveNewMerchant", $"New Merchant MerchantName : {existingMerchant.MerchantName} BusinessName : {existingMerchant.BusinessName}");

                return new ResponseStatus { ResponseStatusId = 200, ResponseMessage = "Merchant Updated Successfully" };
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while updating merchant.");
                return new ResponseStatus { ResponseStatusId = 500, ResponseMessage = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating merchant.");
                return new ResponseStatus { ResponseStatusId = 500, ResponseMessage = $"Internal server error: {ex.Message}" };
            }
        }

        public async Task<ResponseStatus> DeleteMerchantAsync(int merchantId)
        {
            try
            {
                var existingMerchant = await _dbContext.SystemMerchants.FindAsync(merchantId);
                if (existingMerchant == null)
                {
                    return new ResponseStatus { ResponseStatusId = 400, ResponseMessage = "Merchant doesn't exist" };
                }

                _dbContext.SystemMerchants.Remove(existingMerchant);
                await _dbContext.SaveChangesAsync();

                return new ResponseStatus { ResponseStatusId = 200, ResponseMessage = "Merchant Deleted Successfully" };
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while deleting merchant.");
                return new ResponseStatus { ResponseStatusId = 500, ResponseMessage = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting merchant.");
                return new ResponseStatus { ResponseStatusId = 500, ResponseMessage = $"Internal server error: {ex.Message}" };
            }
        }

        // Helper method to map DTO to Entity
        private SystemMerchants MapDtoToEntity(SystemMerchantsDto dto)
        {
            return new SystemMerchants
            {
                MerchantName = dto.MerchantName,
                BusinessName = dto.BusinessName,
                BusinessType = dto.BusinessType,
                BusinessRegistrationNo = dto.BusinessRegistrationNo,
                KRAPIN = dto.KRAPIN,
                BusinessNature = dto.BusinessNature,
                BusinessCategory = dto.BusinessCategory,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                SocialMedia = dto.SocialMedia,
                BankName = dto.BankName,
                BankAccountNo = dto.BankAccountNo,
                BankAccountName = dto.BankAccountName,
                MpesaPaybill = dto.MpesaPaybill,
                MpesaTillNumber = dto.MpesaTillNumber,
                PreferredPaymentChannel = dto.PreferredPaymentChannel,
                KRAPINCertificate = dto.KRAPINCertificate,
                BusinessRegistrationCertificate = dto.BusinessRegistrationCertificate,
                TermsAndCondition = dto.TermsAndCondition,
                ReturnPolicy = dto.ReturnPolicy,
                DeliveryMethod = dto.DeliveryMethod,
                Status = dto.Status
            };
        }

        // Helper method to update existing entity from DTO
        private void UpdateEntityFromDto(SystemMerchants entity, SystemMerchantsDto dto)
        {
            entity.MerchantName = dto.MerchantName;
            entity.BusinessName = dto.BusinessName;
            entity.BusinessType = dto.BusinessType;
            entity.BusinessRegistrationNo = dto.BusinessRegistrationNo;
            entity.KRAPIN = dto.KRAPIN;
            entity.BusinessNature = dto.BusinessNature;
            entity.BusinessCategory = dto.BusinessCategory;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Address = dto.Address;
            entity.SocialMedia = dto.SocialMedia;
            entity.BankName = dto.BankName;
            entity.BankAccountNo = dto.BankAccountNo;
            entity.BankAccountName = dto.BankAccountName;
            entity.MpesaPaybill = dto.MpesaPaybill;
            entity.MpesaTillNumber = dto.MpesaTillNumber;
            entity.PreferredPaymentChannel = dto.PreferredPaymentChannel;
            entity.KRAPINCertificate = dto.KRAPINCertificate;
            entity.BusinessRegistrationCertificate = dto.BusinessRegistrationCertificate;
            entity.TermsAndCondition = dto.TermsAndCondition;
            entity.ReturnPolicy = dto.ReturnPolicy;
            entity.DeliveryMethod = dto.DeliveryMethod;
            entity.Status = dto.Status;
        }
    }
}
