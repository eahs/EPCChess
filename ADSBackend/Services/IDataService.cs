using ADSBackend.Data;
using ADSBackend.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ADSBackend.Models.Identity;

namespace ADSBackend.Services
{
    public interface IDataService
    {
        Task<int> GetCurrentSeasonId();
        Task<SelectList> GetSeasonSelectList(int currentSeasonId);
        Task<int> GetSchoolIdAsync(ClaimsPrincipal User, int seasonId);
        Task<List<Division>> GetDivisionStandingsAsync(int seasonId);
        Task<List<Match>> GetUpcomingMatchesAsync(int seasonId, int schoolId, int count);
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal User);
        Task AddUserToSchoolAsync(ClaimsPrincipal User, int schoolId);
        Task AddUserToSchoolAsync(ApplicationUser User, int schoolId);
        Task<ApplicationUser> GetUserByIdAsync(int userId);

    }
}
