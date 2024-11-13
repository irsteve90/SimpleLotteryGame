using LotteryGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Abstractions
{
    public interface IPlayerManager
    {
        Task<Player> CreatePlayerAsync();
        Task<List<Player>> CreatePlayersAsync(int playerCount);
        Task<Player> UpdatePlayerBalance(int playerId, decimal balance);
    }
}
