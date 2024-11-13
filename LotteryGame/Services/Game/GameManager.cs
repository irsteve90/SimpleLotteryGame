using Ardalis.GuardClauses;
using LotteryGame.Abstractions;
using LotteryGame.Abstractions.Common;
using LotteryGame.Abstractions.Game;
using LotteryGame.Entities;
using LotteryGame.Exceptions;
using LotteryGame.Models;
using LotteryGame.Services.TierPrizes;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace LotteryGame.Services.Game
{
    /// <summary>
    /// The Game Manager orchestrates the whole game
    /// </summary>
    public class GameManager : IGameManager
    {
        private readonly IGameUI _gameUI;
        private readonly ITicketManager _ticketManager;
        private readonly IPlayerManager _playerManager;
        private readonly IGameDbContext _gameDbContext;
        private readonly GameConfig _gameConfig;
        private readonly IEnumerable<TierPrizeBase> _prizeTiers;

        private static readonly Random _random = new Random();
        private GameEngine? gameEngine;

        public GameManager(IOptions<GameConfig> gameConfig, IGameUI gameUI, ITicketManager ticketManager, IPlayerManager playerManager, IGameDbContext gameDbContext, IEnumerable<TierPrizeBase> prizeTiers)
        {
            _gameConfig = Guard.Against.Null(gameConfig?.Value);
            _gameUI = Guard.Against.Null(gameUI);
            _ticketManager = Guard.Against.Null(ticketManager);
            _playerManager = Guard.Against.Null(playerManager);
            _gameDbContext = Guard.Against.Null(gameDbContext);
            _prizeTiers = Guard.Against.Null(prizeTiers);
        }

        /// <summary>
        /// Start The Game Process
        /// </summary>
        /// <returns></returns>
        public async Task StartNewGame()
        {
            // Initialize New Game
            await InitiateNewGame();

            // Create Player 1
            var player1 = await _playerManager.CreatePlayerAsync();

            // Run Game UI
            await StartGameUI(player1);
        }

        /// <summary>
        /// Initiates a new game by reconfiguring the engine and creating a new instance.
        /// </summary>
        /// <returns></returns>
        private async Task InitiateNewGame()
        {
            var gameEntity = new GameInstance() { StartDate = DateTime.UtcNow };
            _gameDbContext.GameInstances.Add(gameEntity);
            await _gameDbContext.SaveChangesAsync(CancellationToken.None);
            gameEngine = new GameEngine(gameEntity, _gameConfig, _prizeTiers);
        }

        /// <summary>
        /// Starts the UI Messaging/Interactive process for a user.
        /// </summary>
        /// <param name="player1"></param>
        /// <returns></returns>
        private async Task StartGameUI(Player player1)
        {
            // Generate CPU Players
            var cpuPlayers = await _playerManager.CreatePlayersAsync(_random.Next(_gameConfig.MinPlayers, _gameConfig.MaxPlayers));

            // Start and Populate UI
            _gameUI.ShowWelcomeMessage(player1);
            _gameUI.ShowNewLine();

            // Request Tickets for Player 1
            int requestedTickets = _gameUI.RequestTicketsAmount(player1);

            // Try Buy Ticket Amount, Exit on Insufficient Funds
            try
            {
                await _ticketManager.BuyTicketsAsync(player1.Id, gameEngine.GameInstance.Id, requestedTickets);
            }
            catch(InsufficientFundsException ex)
            {
                _gameUI.ShowMessage(ex.Message);
                Environment.Exit(0);
            }

            // Generate Tickets for CPU Players
            var ticketTasks = cpuPlayers.Select(player =>
                _ticketManager.BuyTicketsAsync(player.Id, gameEngine.GameInstance.Id, _random.Next(_gameConfig.MinTickets, _gameConfig.MaxTickets + 1)));
            await Task.WhenAll(ticketTasks);

            // Populate UI With Whos Playing
            _gameUI.ShowNewLine();
            _gameUI.ShowsOtherPlayersInGameMessage(cpuPlayers.Count);
            _gameUI.ShowNewLine();

            // Retrieve All Tickets Assigned to Game
            var allTickets = await _ticketManager.GetTicketsForGameAsync(gameEngine.GameInstance.Id);

            // Draw Tickets
            DrawResult results = gameEngine.DrawWinners(allTickets);

            // Update Game Revenue
            await UpdateHouseRevenue(results.HouseRevenue);

            // Update All User Balances
            var updatedWinningPlayers = await UpdateUserBalancesFromDraw(results);

            // Update Player 1 state if Won
            player1 = updatedWinningPlayers?.FirstOrDefault(p => p?.Id == player1?.Id) ?? player1;

            // Draw and Show Results
            _gameUI.ShowDrawResultsMessage(results);
            _gameUI.ShowNewLine();
            _gameUI.ShowHouseRevenueMessage(results.HouseRevenue);
            _gameUI.ShowNewLine();

            _gameUI.ShowRemainingBalanceMessage(player1.PlayerBalance);
            _gameUI.ShowNewLine();

            if (_gameUI.RequestRestartGameMenu())
                await RestartGame(player1);
        }

        /// <summary>
        /// Updates a Games House Revenue
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task UpdateHouseRevenue(decimal amount)
        {
            var game = await _gameDbContext.GameInstances.FindAsync(gameEngine.GameInstance.Id)
                ?? throw new InvalidOperationException("Game not found.");

            game.HouseRevenue += amount;
            await _gameDbContext.SaveChangesAsync(CancellationToken.None);
        }

        /// <summary>
        /// Use Draw results to Update Players Balances
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private async Task<List<Player>> UpdateUserBalancesFromDraw(DrawResult results)
        {
            // Group All winnings tickets by player id and sum of revenue earned.
            Dictionary<int, decimal> playerRevenues = results.TierResults
                .SelectMany(tr => tr.Value.TicketsWon, (tr, ticket) => new { ticket.PlayerId, tr.Value.RevenuePerTicket })
                .GroupBy(x => x.PlayerId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.RevenuePerTicket)
                );

            // Update all player balances, Ideally this would have been batched
            var updateBalancesTask = playerRevenues.Select(revenue =>
                _playerManager.UpdatePlayerBalance(revenue.Key, revenue.Value));
            var updatedPlayers = await Task.WhenAll(updateBalancesTask);
            return updatedPlayers.ToList();
        }

        /// <summary>
        /// Closes currentGame and Resets State of Game 
        /// </summary>
        /// <param name="player1"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task RestartGame(Player player1)
        {
            // Save End Date
            var game = await _gameDbContext.GameInstances.FindAsync(gameEngine.GameInstance.Id)
                ?? throw new InvalidOperationException("Game not found.");

            game.EndDate = DateTime.UtcNow;
            await _gameDbContext.SaveChangesAsync(CancellationToken.None);
            gameEngine = null;

            // Clear Console
            _gameUI.ClearMessages();

            // Initiate New Game
            await InitiateNewGame();
            await StartGameUI(player1);
        }
    }
}
