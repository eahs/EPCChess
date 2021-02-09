using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models.PlayhubViewModels;
using Microsoft.AspNetCore.SignalR;

namespace ADSBackend.Hubs
{
    public class GameHub : Hub
    {
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

        public async Task SendMessage(string user, string message)
        {
            GameJson game = new GameJson();

            await Clients.All.SendAsync("UpdateMatches", game);
        }
    }
}
