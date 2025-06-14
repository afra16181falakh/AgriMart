using AgriMartAPI.Models;
using System;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetUserProfile(Guid userId);
        Task<bool> UpdateUserProfile(UserProfile userProfile);
        // Note: Creating a user profile is often part of a larger "user registration"
        // service, so we might not need a separate Create method here.
    }
}