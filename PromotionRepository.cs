using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly string? _connectionString;

        public PromotionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotions()
        {
            var promotions = new List<Promotion>();
            // Your logic to get promotions where IsActive = true and dates are valid
            // Add actual ADO.NET query here later
            await Task.CompletedTask; // Suppress CS1998 warning for now
            return promotions;
        }
    }
}