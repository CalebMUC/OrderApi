using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.Models;
using Minimart_Api.Repositories.AddressesRepo;

public class AddressRepo : IAddressRepo
{
    private readonly MinimartDBContext _dbContext;
    private readonly ILogger<AddressRepo> _logger;

    public AddressRepo(MinimartDBContext dBContext, ILogger<AddressRepo> logger)
    {
        _dbContext = dBContext;
        _logger = logger;
    }

    public async Task<OperationResult> AddAddressAsync(AddressDTO address)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Handle default address logic
            if (address.isDefault)
            {
                await ResetExistingDefaultAddress(address.UserID);
            }

            var newAddress = new Addresses
            {
                UserID = address.UserID,
                Name = address.Name?.Trim(),
                Phonenumber = address.Phonenumber,
                PostalAddress = address.PostalAddress.Trim(),
                County = address.County.Trim(),
                Town = address.Town.Trim(),
                PostalCode = address.PostalCode?.Trim(),
                ExtraInformation = address.ExtraInformation?.Trim(),
                isDefault = address.isDefault,
                CreatedOn = DateTime.UtcNow,
                LastUpdatedOn = DateTime.UtcNow
            };

            await _dbContext.Addresses.AddAsync(newAddress);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return OperationResult.Success(newAddress.AddressID);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error adding address to database");
            return OperationResult.Failure("Database error while adding address");
        }
    }

    public async Task<OperationResult> EditAddressAsync(EditAddressDTO address)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var existingAddress = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.AddressID == address.AddressID);


            if (existingAddress == null)
                return OperationResult.Failure("Address not found");

            // Handle default address logic
            if (address.isDefault)
            {
                await ResetExistingDefaultAddress(address.UserID, address.AddressID);
            }

            // Update fields
            existingAddress.Name = address.Name?.Trim();
            existingAddress.Phonenumber = address.Phonenumber;
            existingAddress.PostalAddress = address.PostalAddress.Trim();
            existingAddress.County = address.County.Trim();
            existingAddress.Town = address.Town.Trim();
            existingAddress.PostalCode = address.PostalCode?.Trim();
            existingAddress.ExtraInformation = address.ExtraInformation?.Trim();
            existingAddress.isDefault = address.isDefault;
            existingAddress.LastUpdatedOn = DateTime.UtcNow;

            _dbContext.Addresses.Update(existingAddress);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return OperationResult.Success("Address updated successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating address");
            return OperationResult.Failure("Database error while updating address");
        }
    }

    private async Task ResetExistingDefaultAddress(int userId, int? excludeAddressId = null)
    {
        var query = _dbContext.Addresses
            .Where(a => a.UserID == userId && a.isDefault);

        if (excludeAddressId.HasValue)
        {
            query = query.Where(a => a.AddressID != excludeAddressId.Value);
        }

        var defaultAddresses = await query.ToListAsync();
        foreach (var addr in defaultAddresses)
        {
            addr.isDefault = false;
        }

        if (defaultAddresses.Any())
        {
            _dbContext.Addresses.UpdateRange(defaultAddresses);
        }
    }

    public async Task<Addresses> GetAddressByIdAsync(int addressId)
    {
        return await _dbContext.Addresses.FindAsync(addressId);
    }

    public async Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(int userId)
    {
        return await _dbContext.Addresses
            .Where(a => a.UserID == userId)
            .Join(_dbContext.Counties,
                address => address.County,       // Join on County name from Address
                county => county.CountyName,      // Join on CountyName from County
                (address, county) => new { Address = address, County = county })
            .Join(_dbContext.Towns,
                combined => combined.Address.Town, // Join on Town name from Address
                town => town.TownName,             // Join on TownName from Town
                (combined, town) => new GetAddressDTO
                {
                    AddressID = combined.Address.AddressID,
                    UserID = combined.Address.UserID,
                    Name = combined.Address.Name,
                    PhoneNumber = combined.Address.Phonenumber,
                    PostalAddress = combined.Address.PostalAddress,
                    County = combined.Address.County,
                    CountyId = combined.County.CountyId,  // From County table
                    Town = combined.Address.Town,
                    TownId = town.TownId,                 // From Town table
                    PostalCode = combined.Address.PostalCode,
                    ExtraInformation = combined.Address.ExtraInformation,
                    isDefault = combined.Address.isDefault
                })
            .ToListAsync();
    }
}