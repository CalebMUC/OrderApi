﻿using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.Deliveries
{
    public class DeliveriesRepo
    {
        private readonly MinimartDBContext _dbContext;
        private readonly ILogger<Categories> _logger;

        public DeliveriesRepo(MinimartDBContext dBContext, ILogger<Categories> logger)
        {
            _dbContext = dBContext;
            _logger = logger;
        }
        public async Task<IEnumerable<Counties>> GetAllCountiesAsync()
        {
            return await _dbContext.Counties.ToListAsync();
        }

        public async Task<IEnumerable<Towns>> GetTownsByCountyAsync(int countyId)
        {
            return await _dbContext.Towns.Where(t => t.CountyId == countyId).ToListAsync();
        }

        public async Task<IEnumerable<DeliveryStations>> GetDeliveryStationsByTownAsync(int townId)
        {
            return await _dbContext.DeliveryStations.Where(ds => ds.TownId == townId).ToListAsync();
        }
    }
}
