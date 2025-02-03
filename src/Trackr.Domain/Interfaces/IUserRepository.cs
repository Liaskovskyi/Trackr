using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;

namespace Trackr.Domain.Interfaces
{
    public interface IUserRepository
    {
        //public Task<string> GetAuthTokenAsync(int id, AuthProvider providerName);
        public Task<bool> RegisterUserAsync(User registerInfo); 
    }
}
