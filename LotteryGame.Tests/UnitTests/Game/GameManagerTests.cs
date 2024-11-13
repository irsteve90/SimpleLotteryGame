using LotteryGame.Abstractions.Common;
using LotteryGame.Abstractions.Game;
using LotteryGame.Abstractions;
using LotteryGame.Entities;
using LotteryGame.Exceptions;
using LotteryGame.Models;
using LotteryGame.Services.Game;
using LotteryGame.Services.TierPrizes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MockQueryable.Moq;

namespace LotteryGame.Tests.UnitTests.Game
{
    [TestFixture]
    public class GameManagerTests
    {
        private Mock<IGameUI> _mockGameUI;
        private Mock<ITicketManager> _mockTicketManager;
        private Mock<IPlayerManager> _mockPlayerManager;
        private Mock<IGameDbContext> _mockGameDbContext;
        private Mock<IOptions<GameConfig>> _mockGameConfigOptions;
        private GameManager _gameManager;

        [SetUp]
        public void SetUp()
        {
            _mockGameUI = new Mock<IGameUI>();
            _mockTicketManager = new Mock<ITicketManager>();
            _mockPlayerManager = new Mock<IPlayerManager>();
            _mockGameDbContext = new Mock<IGameDbContext>();
            _mockGameConfigOptions = new Mock<IOptions<GameConfig>>();

            var mockGames = new List<GameInstance>().AsQueryable().BuildMockDbSet();
            _mockGameDbContext.Setup(db => db.GameInstances).Returns(mockGames.Object);
            _mockGameDbContext.Setup(db => db.GameInstances.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new GameInstance() { });

            _mockGameConfigOptions.Setup(config => config.Value).Returns(new GameConfig
            {
                MinPlayers = 1,
                MaxPlayers = 10,
                MinTickets = 10,
                MaxTickets = 15,
                TicketPrice = 1.00m,
                DefaultPlayerBalance = 10m
            });

            _gameManager = new GameManager(
                _mockGameConfigOptions.Object,
                _mockGameUI.Object,
                _mockTicketManager.Object,
                _mockPlayerManager.Object,
                _mockGameDbContext.Object,
                new List<TierPrizeBase>()
                {
                    new GrandTierPrize(),
                    new SecondTierPrize(),
                    new ThirdTierPrize()
                }
            );
        }

        [Test]
        public async Task StartNewGame_ShouldInitiateNewGameAndStartUI()
        {
            // Arrange
            var players = Enumerable.Range(1, _mockGameConfigOptions.Object.Value.MaxPlayers)
                .Select(i => new Player { Id = i, PlayerBalance = _mockGameConfigOptions.Object.Value.DefaultPlayerBalance })
                .ToList();

            _mockPlayerManager
                .Setup(x => x.CreatePlayerAsync())
                .ReturnsAsync(players.First());

            _mockPlayerManager.Setup(x => x.CreatePlayersAsync(It.IsAny<int>()))
                .ReturnsAsync(players.Skip(1).ToList());

            _mockGameUI.Setup(x => x.RequestTicketsAmount(It.IsAny<Player>()))
                .Returns(5);

            _mockGameUI.Setup(x => x.RequestRestartGameMenu())
                .Returns(false);

            var tickets = Enumerable.Range(1, 20)
                .Select(i => new Ticket { PlayerId = (i - 1) % 10 + 1 })
                .ToList();

            _mockTicketManager.Setup(x => x.GetTicketsForGameAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tickets.ToList());

            // Act
            await _gameManager.StartNewGame();

            // Assert
            _mockPlayerManager.Verify(p => p.CreatePlayersAsync(It.IsAny<int>()), Times.Once);
            _mockPlayerManager.Verify(p => p.CreatePlayerAsync(), Times.Once);
            _mockGameUI.Verify(p => p.RequestTicketsAmount(It.IsAny<Player>()), Times.Once);
            _mockTicketManager.Verify(p => p.BuyTicketsAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(players.Count));
            _mockGameUI.Verify(p => p.RequestTicketsAmount(It.IsAny<Player>()), Times.Once);
            _mockGameUI.Verify(p => p.ShowDrawResultsMessage(It.IsAny<DrawResult>()), Times.Once);
        } 
    }
}