using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(string userId, string email, string name, IList<string> roles);
        string GenerateRefreshToken();
        Guid? ValidateAccessToken(string token);
    }
}