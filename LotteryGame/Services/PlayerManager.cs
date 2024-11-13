using Ardalis.GuardClauses;
using LotteryGame.Abstractions.Common;
using LotteryGame.Abstractions;
using LotteryGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotteryGame.Models;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace LotteryGame.Services
{
    /// <summary>
    /// Player Service, Supports In Creating of New Players
    /// </summary>
    public class PlayerManager : IPlayerManager
    {
        private readonly IGameDbContext _gameDbContext;
        private readonly GameConfig _gameConfig;

        public PlayerManager(IGameDbContext gameDbContext, IOptions<GameConfig> gameConfig)
        {
            _gameDbContext = Guard.Against.Null(gameDbContext, nameof(gameDbContext));
            _gameConfig = Guard.Against.Null(gameConfig?.Value);
        }

        /// <summary>
        /// Create New Player
        /// </summary>
        /// <returns></returns>
        public async Task<Player> CreatePlayerAsync()
        {
            var players = await CreatePlayersAsync(1);
            return players.First();
        }

        /// <summary>
        /// Create New Players
        /// </summary>
        /// <param name="playerCount"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<List<Player>> CreatePlayersAsync(int playerCount)
        {
            if (playerCount <= 0)
            {
                throw new ArgumentException("The number of players must be greater than zero.", nameof(playerCount));
            }

            var players = Enumerable.Range(1, playerCount)
                .Select(x => new Player { PlayerBalance = _gameConfig.DefaultPlayerBalance }).ToList();

            _gameDbContext.Players.AddRange(players);
            await _gameDbContext.SaveChangesAsync(CancellationToken.None);

            return players;
        }

        /// <summary>
        /// Update Players Balance
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Player> UpdatePlayerBalance(int playerId, decimal amount)
        {
            // Find the player in the database
            var player = await _gameDbContext.Players.FindAsync(playerId);
            if (player == null)
            {
                throw new InvalidOperationException("Player not found.");
            }

            // Update the player's balance
            player.PlayerBalance += amount;

            // Throw if Negitive Value
            if (player.PlayerBalance < 0)
                throw new InvalidOperationException("Player balance cannot be negative.");

            // Save the changes to the database
            await _gameDbContext.SaveChangesAsync(CancellationToken.None);

            return player;
        }
    }
}
