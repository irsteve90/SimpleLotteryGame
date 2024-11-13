using Ardalis.GuardClauses;
using LotteryGame.Abstractions;
using LotteryGame.Abstractions.Common;
using LotteryGame.Entities;
using LotteryGame.Exceptions;
using LotteryGame.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LotteryGame.Services
{
    /// <summary>
    /// Ticket Manager Service, Handles Creating and Requesting Of Tickets
    /// </summary>
    public class TicketManager : ITicketManager
    {
        private readonly IGameDbContext _gameDbContext;
        private readonly GameConfig _gameConfig;
        private readonly IPlayerManager _playerManager;

        public TicketManager(IGameDbContext gameDbContext, IOptions<GameConfig> gameConfig, IPlayerManager playerManager)
        {
            _gameDbContext = Guard.Against.Null(gameDbContext);
            _gameConfig = Guard.Against.Null(gameConfig?.Value);
            _playerManager = Guard.Against.Null(playerManager);
        }

        /// <summary>
        /// Allow User To Buy Tickets For Game
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="gameId"></param>
        /// <param name="requestedTickets"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<List<Ticket>> BuyTicketsAsync(int playerId, Guid gameId, int requestedTickets)
        {
            // Check If Player Exists
            var player = await _gameDbContext.Players.FindAsync(playerId)
                ?? throw new InvalidOperationException("Player not found.");

            // Check if Player Can Afford A Singular Ticket
            if (player.PlayerBalance < _gameConfig.TicketPrice)
            {
                throw new InsufficientFundsException(_gameConfig.TicketPrice, player.PlayerBalance);
            }

            // Convert To Number of Tickets User Can Buy.
            int ticketsAffordable = (int)(player.PlayerBalance / _gameConfig.TicketPrice);

            // Generate Ticket Entities
            var ticketEntities = Enumerable.Range(1, Math.Min(requestedTickets, ticketsAffordable))
                .Select(x => new Ticket { PlayerId = playerId, GameId = gameId, PurchaseDate = DateTime.UtcNow }).ToList();

            // Deduct Player Balance
            await _playerManager.UpdatePlayerBalance(playerId, -(ticketEntities.Count() * _gameConfig.TicketPrice));

            // Commit and Save Tickets
            _gameDbContext.Tickets.AddRange(ticketEntities);
            await _gameDbContext.SaveChangesAsync(CancellationToken.None);

            return ticketEntities;
        }

        /// <summary>
        /// Get All Tickets Against Game Id
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task<List<Ticket>> GetTicketsForGameAsync(Guid gameId)
        {
            return await _gameDbContext.Tickets.Where(t => t.GameId == gameId).ToListAsync();
        }
    }
}
