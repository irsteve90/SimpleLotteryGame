using Ardalis.GuardClauses;
using LotteryGame.Entities;
using LotteryGame.Models;
using LotteryGame.Services.TierPrizes;

namespace LotteryGame.Services.Game
{
    /// <summary>
    /// Game Engine, Core Logic to Handle Game Mechanics
    /// </summary>
    public class GameEngine
    {
        private readonly List<TierPrizeBase> _prizeTiers;
        private readonly GameConfig _gameConfig;
        public GameInstance GameInstance { get; init; }

        public GameEngine(GameInstance gameInstance, GameConfig gameConfig, IEnumerable<TierPrizeBase> prizeTiers)
        {
            GameInstance = Guard.Against.Null(gameInstance);
            _gameConfig = Guard.Against.Null(gameConfig);
            _prizeTiers = Guard.Against.NullOrEmpty(prizeTiers, nameof(prizeTiers)).ToList();
        }

        /// <summary>
        /// Method in drawing the winning tickets
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public DrawResult DrawWinners(List<Ticket> tickets)
        {
            // Validate If Enough Players
            var playerCount = tickets.Select(x => x.PlayerId).Distinct().Count();
            if (playerCount < _gameConfig.MinPlayers || playerCount > _gameConfig.MaxPlayers)
                    throw new InvalidOperationException($"There should only be between {_gameConfig.MinPlayers} and {_gameConfig.MaxPlayers} playing at one time.");

            // Calculate Ticket Revenue
            var ticketRevenue = tickets.Count * _gameConfig.TicketPrice;

            var tierResults = new Dictionary<TierPrizeBase, TierResult>();
            decimal winningsRevenue = 0m;

            foreach (var tier in _prizeTiers)
            {
                // Select winning tickets for the current tier
                var winners = tier.SelectWinners(tickets);
                if (winners.Count == 0) continue;

                // Calculate the tier revenue and revenue per ticket
                decimal tierRevenueShare = tier.SharePercentage / 100m;
                decimal tierRevenue = ticketRevenue * tierRevenueShare;
                decimal revenuePerTicket = Math.Round(tierRevenue / winners.Count, 2, MidpointRounding.AwayFromZero);

                // Update tier results and total winnings revenue
                tierResults[tier] = new TierResult() { TicketsWon = winners, RevenuePerTicket = revenuePerTicket };
                winningsRevenue += winners.Count * revenuePerTicket;

                // Exclude already won tickets
                tickets = tickets.Except(winners).ToList();
            }

            // Calculate house revenue
            var houseRevenue = ticketRevenue - winningsRevenue;

            return new DrawResult() { TierResults = tierResults, HouseRevenue = houseRevenue };
        }
    }
}
