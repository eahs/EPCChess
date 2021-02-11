using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models.Identity;

namespace ADSBackend.Services
{
    public interface ITokenRefresher
    {
        Task<bool> RefreshTokens(ApplicationUser user, bool signInUser = false);
    }
}
