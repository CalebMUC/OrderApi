using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS.Merchants;
using Minimart_Api.Services.SignalR;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories.Merchants
{
    public class MerchantRepo : IMerchantRepo
    {
        private readonly MinimartDBContext _dbContext;
        private readonly IHubContext<ActivityHub> _hubContext;
        public MerchantRepo(MinimartDBContext dBContext, IHubContext<ActivityHub> hubContext)
        {

            _dbContext = dBContext;
            _hubContext = hubContext;
        }

        public async Task<IList<Merchants>> GetMerchantsAsync()
        {

            return await _dbContext.Merchants.ToListAsync();
        }
        public async Task<IList<BusinessTypes>> GetBusinessTypes()
        {

            return await _dbContext.BusinessTypes.ToListAsync();
        }
        public async Task<ResponseStatus> AddMerchants(MerchantDTO merchantDTO)
        {
            try
            {
                //Check if Merchant Already Exists
                var existingMerchant = await _dbContext.Merchants
              .FirstOrDefaultAsync(x => x.Name == merchantDTO.MerchantName);
                if (existingMerchant != null)
                {
                    return new ResponseStatus
                    {
                        ResponseStatusId = 409,
                        ResponseMessage = $"Product '{merchantDTO.MerchantName}' Already Exists"
                    };
                }
                var newMerchant = new Merchants
                {
                    Name = merchantDTO.MerchantName,
                    Description = merchantDTO.Description,
                    BusinessType = merchantDTO.BusinessType,
                    Category = merchantDTO.Category,
                    County = merchantDTO.County,
                    Town = merchantDTO.Town,
                    ExtraInformation = merchantDTO.ExtraInformation,
                    Email = merchantDTO.Email,
                    Phonenumber = merchantDTO.Phonenumber,
                    CreatedBy = merchantDTO.CreatedBy,
                    CreatedOn = DateTime.UtcNow

                };

                await _dbContext.AddAsync(newMerchant);

                await _dbContext.SaveChangesAsync();

                _hubContext.Clients.All.SendAsync("ReceiveNewMerchant", $"New Merchant MerchantName : {newMerchant.Name} BusinessName : {newMerchant.BusinessType}");

                return new ResponseStatus
                {
                    ResponseStatusId = 200,
                    ResponseMessage = "Merchant Saved Successfully"

                };
            }
            catch (Exception ex)
            {
                return new ResponseStatus
                {
                    ResponseStatusId = 500,
                    ResponseMessage = "Internal Server ERROR"

                };
            }


        }
    }
}
