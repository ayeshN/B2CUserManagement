using B2CUserManagement.Models;
using System.Threading.Tasks;

namespace B2CUserManagement.Interfaces
{
    public interface IUserManager
    {
        Task CreateUser(B2CUser user);

        void DeleteUser(string email);
    }
}