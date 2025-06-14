using AgriMartAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface IAddressRepository
    {
        // Renamed to match the controller call (singular "Address")
        Task<IEnumerable<Address>> GetAddressByUserId(Guid userId);

        // Changed id type from int to Guid to match Address model
        Task<Address?> GetAddressById(Guid id);

        Task<Address> CreateAddress(Address address);

        Task<bool> UpdateAddress(Address address);

        // Changed id type from int to Guid to match Address model
        Task<bool> DeleteAddress(Guid id);
    }
}