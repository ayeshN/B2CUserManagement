using B2CUserManagement.Models;
using Microsoft.Graph;
using System.Threading.Tasks;

namespace B2CUserManagement.Interfaces
{
    public interface IUserManager
    {
        Task CreateUser(B2CUser user);

        Task<IGraphServiceUsersCollectionPage> GetUserByEmail(string email);

        Task<bool> DeleteUser(string email);
    }
}