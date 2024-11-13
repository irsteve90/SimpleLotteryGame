using LotteryGame.Entities;
using LotteryGame.Models;
using LotteryGame.Services.Game;
using LotteryGame.Services.TierPrizes;
using Microsoft.Extensions.Options;
using Moq;

namespace LotteryGame.Tests.UnitTests.Game
{
    [TestFixture]
    public class GameEngineTests
    {
        private Mock<IOptions<GameConfig>> _mockGameConfig;
        private IList<TierPrizeBase> _tierPrizes;
        private GameEngine _gameEngine;
        private GameConfig _gameConfig;

        [SetUp]
        public void SetUp()
        {
            _tierPrizes = new List<TierPrizeBase>()
            {
                new TestTierPrizeClass(),
                new TestTierPrizeClass(),
                new TestTierPrizeClass()
            };

            _gameConfig = new GameConfig
            {
                MinPlayers = 1,
                MaxPlayers = 10,
                MinTickets = 10,
                MaxTickets = 15,
                TicketPrice = 1.33m
            };
            _mockGameConfig = new Mock<IOptions<GameConfig>>();
            _mockGameConfig.Setup(x => x.Value).Returns(_gameConfig);

            _gameEngine = new GameEngine(new GameInstance(), _mockGameConfig.Object.Value, _tierPrizes);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenGameConfigIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GameEngine(null, _mockGameConfig.Object.Value, new List<TierPrizeBase>()));
        }

        [Test]
        public void DrawWinners_ShouldThrowInvalidOperationException_WhenNotEnoughPlayers()
        {
            // Arrange
            var tickets = Enumerable.Range(1, 20)
                .Select(playerId => new Ticket { PlayerId = playerId })
                .ToList();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _gameEngine.DrawWinners(tickets));
            Assert.That(exception.Message, Is.EqualTo($"There should only be between {_gameConfig.MinPlayers} and {_gameConfig.MaxPlayers} playing at one time."));
        }

        [Test]
        public void DrawWinners_ShouldDistributeWinningsAndCalculateHouseRevenueCorrectly()
        {
            // Arrange
            var tickets = Enumerable.Range(1, 90)
                .Select(i => new Ticket { PlayerId = (i - 1) % 10 + 1 })
                .ToList();

            // Act
            var result = _gameEngine.DrawWinners(tickets);

            decimal totalTicketRevenue = tickets.Count * _gameConfig.TicketPrice;
            decimal wonTicketRevenueSum = result.TierResults.Sum(x => x.Value.TicketsWon.Count * x.Value.RevenuePerTicket);
            decimal expectedHouseRevenue = totalTicketRevenue - wonTicketRevenueSum;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TierResults.Count, Is.EqualTo(_tierPrizes.Count), "There should be results for three tiers.");
            Assert.That(result.HouseRevenue, Is.EqualTo(expectedHouseRevenue), $"The expected house revenue is {expectedHouseRevenue}");
        }


        /// <summary>
        /// If the number of winners for a prize tier is not exactly divisible by the number of winners of 
        /// that tier, the closest equal split should be calculated, and any remaining amount should be added to
        /// the house profit.
        /// </summary>
        /// <returns></returns>
        [Test]
        public void DrawWinners_ShouldAccuratelyDiviseWinningsAndAddRemainderAsHouseRevenue()
        {
            // Arrange
            var tickets = Enumerable.Range(1, 3)
                .Select(i => new Ticket { PlayerId = (i - 1) % 10 + 1 })
                .ToList();

            var gameTiers = new List<TierPrizeBase>()
            {
                new TestTierPrizeClassFullReturn(),
            };

            // Reinitialize GameEngine with these mocked tiers
            _gameEngine = new GameEngine(new GameInstance(), _gameConfig, gameTiers);

            // Act
            var result = _gameEngine.DrawWinners(tickets);
            decimal totalTicketRevenue = tickets.Count * _gameConfig.TicketPrice;
            decimal wonTicketRevenueSum = result.TierResults.Sum(x => x.Value.TicketsWon.Count * x.Value.RevenuePerTicket);
            decimal expectedHouseRevenue = totalTicketRevenue - wonTicketRevenueSum;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TierResults.Count, Is.EqualTo(1), "There should be results for three tiers.");
            Assert.That(result.HouseRevenue, Is.EqualTo(expectedHouseRevenue), $"The expected house revenue is {expectedHouseRevenue}");
        }
    }
}
