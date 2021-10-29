using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.Identity;
using ADSBackend.Models.PlayhubViewModels;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ADSBackend.Hubs
{
    public class GameHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public GameHub(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task JoinMatch(string matchId)
        {
            string matchGroup = $"match_{matchId}";

            await Groups.AddToGroupAsync(Context.ConnectionId, matchGroup);
        }

        public async Task LeaveMatch(string matchId)
        {
            string matchGroup = $"match_{matchId}";

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, matchGroup);
        }

        public async Task SendMessage(string matchId, string message)
        {
            try
            {
                int mid = -1;
                message ??= "";

                if (Int32.TryParse(matchId, out mid))
                {
                    var match = await _context.Match.FirstOrDefaultAsync(m => m.MatchId == mid);

                    if (Context.User.Identity.IsAuthenticated && match is not null)
                        {
                            if (message.Length > 0)
                            {
                                var filter = new ProfanityFilter.ProfanityFilter();

                                var user = await _userManager.GetUserAsync(Context.User);

                                if (user is not null)
                                {
                                    message = filter.CensorString(message);
                                    message = message.Truncate(255);

                                    MatchChat matchChat = new MatchChat
                                    {
                                        UserId = user.Id,
                                        MatchId = match.MatchId,
                                        Message = message
                                    };

                                    _context.MatchChat.Add(matchChat);
                                    await _context.SaveChangesAsync();

                                    await Clients.Groups("match_" + match.MatchId).SendAsync("ReceiveMessage", user.FullName, message);
                                }
                            }

                        }

                    }

            }
            catch
            { }

        }
    }
}
