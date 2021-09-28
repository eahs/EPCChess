using ADSBackend.Data;
using ADSBackend.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    interface IDataService
    {
        Task<int> GetCurrentSeasonId();
        Task<SelectList> GetSeasonSelectList(int currentSeasonId);
        Task<int> GetSchoolIdAsync(ClaimsPrincipal User, int seasonId);
        Task<List<Division>> GetDivisionStandingsAsync(int seasonId);
        Task<List<Match>> GetUpcomingMatchesAsync(int seasonId, int schoolId, int count);

    }
}
