using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models.Database;

namespace Trackr.Application.Interfaces
{
    public interface IJwtGenerator
    {
        //public string GenerateJWTToken(User user);
        //public string GenerateAccessJWT(User user);
        public string GenerateAccessJWT(string user);
        public string GenerateRefreshJWT(User user);
    }
}
