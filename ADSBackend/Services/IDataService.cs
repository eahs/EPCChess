
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
    /// <summary>
    /// Interface for a service that provides data access and manipulation logic.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Gets the ID of the current season.
        /// </summary>
        /// <returns>The ID of the current season.</returns>
        Task<int> GetCurrentSeasonId();
        /// <summary>
        /// Gets a select list of all seasons.
        /// </summary>
        /// <param name="currentSeasonId">The ID of the currently selected season.</param>
        /// <returns>A <see cref="SelectList"/> of seasons.</returns>
        Task<SelectList> GetSeasonSelectList(int currentSeasonId);
        /// <summary>
        /// Gets the school ID for a user in a specific season.
        /// </summary>
        /// <param name="User">The claims principal of the user.</param>
        /// <param name="seasonId">The ID of the season.</param>
        /// <returns>The school ID, or -1 if not found.</returns>
        Task<int> GetSchoolIdAsync(ClaimsPrincipal User, int seasonId);
        /// <summary>
        /// Gets the standings for all divisions in a specific season.
        /// </summary>
        /// <param name="seasonId">The ID of the season.</param>
        /// <returns>A list of <see cref="Division"/>s with their school standings.</returns>
        Task<List<Division>> GetDivisionStandingsAsync(int seasonId);
        /// <summary>
        /// Gets a list of upcoming matches for a school in a specific season.
        /// </summary>
        /// <param name="seasonId">The ID of the season.</param>
        /// <param name="schoolId">The ID of the school.</param>
        /// <param name="count">The maximum number of matches to return.</param>
        /// <returns>A list of upcoming <see cref="Match"/>es.</returns>
        Task<List<Match>> GetUpcomingMatchesAsync(int seasonId, int schoolId, int count);
        /// <summary>
        /// Gets a user with their associated school data.
        /// </summary>
        /// <param name="User">The claims principal of the user.</param>
        /// <returns>The <see cref="ApplicationUser"/> with included navigation properties.</returns>
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal User);
        /// <summary>
        /// Adds a user to a school.
        /// </summary>
        /// <param name="User">The claims principal of the user.</param>
        /// <param name="schoolId">The ID of the school.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddUserToSchoolAsync(ClaimsPrincipal User, int schoolId);
        /// <summary>
        /// Adds a user to a school.
        /// </summary>
        /// <param name="User">The application user.</param>
        /// <param name="schoolId">The ID of the school.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddUserToSchoolAsync(ApplicationUser User, int schoolId);
        /// <summary>
        /// Gets a user by their ID, including their school associations.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The <see cref="ApplicationUser"/> with included navigation properties.</returns>
        Task<ApplicationUser> GetUserByIdAsync(int userId);
        /// <summary>
        /// Removes a user from a school.
        /// </summary>
        /// <param name="User">The claims principal of the user.</param>
        /// <param name="schoolId">The ID of the school.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveUserFromSchool(ClaimsPrincipal User, int schoolId);
        /// <summary>
        /// Removes a user from a school.
        /// </summary>
        /// <param name="User">The application user.</param>
        /// <param name="schoolId">The ID of the school.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveUserFromSchool(ApplicationUser User, int schoolId);
        /// <summary>
        /// Gets a match by its ID for a specific season, optionally filtered by school.
        /// </summary>
        /// <param name="id">The ID of the match.</param>
        /// <param name="seasonId">The ID of the season.</param>
        /// <param name="schoolId">Optional: The ID of the school to filter by.</param>
        /// <returns>The <see cref="Match"/> with included navigation properties, or null if not found.</returns>
        Task<Match> GetMatchAsync(int? id, int seasonId, int schoolId = -1);

    }
}