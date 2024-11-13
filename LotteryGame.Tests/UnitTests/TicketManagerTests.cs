using LotteryGame.Abstractions;
using LotteryGame.Abstractions.Common;
using LotteryGame.Entities;
using LotteryGame.Exceptions;
using LotteryGame.Models;
using LotteryGame.Services;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Tests.UnitTests
{
    [TestFixture]
    public class TicketManagerTests
    {
        private Mock<IGameDbContext> _mockGameDbContext;
        private Mock<IPlayerManager> _mockPlayerManager;
        private Mock<IOptions<GameConfig>> _mockGameConfig;
        private TicketManager _ticketManager;

        [SetUp]
        public void SetUp()
        {
            _mockGameDbContext = new Mock<IGameDbContext>();
            _mockPlayerManager = new Mock<IPlayerManager>();

            var mockTickets = new List<Ticket>().AsQueryable()
                .BuildMockDbSet();
            _mockGameDbContext.Setup(db => db.Tickets).Returns(mockTickets.Object);

            var mockPlayers = new List<Player>().AsQueryable()
                .BuildMockDbSet();
            _mockGameDbContext.Setup(db => db.Players).Returns(mockPlayers.Object);

            _mockGameConfig = new Mock<IOptions<GameConfig>>();
            _mockGameConfig.Setup(config => config.Value).Returns(new GameConfig
            {
                TicketPrice = 10m
            });

            _ticketManager = new TicketManager(_mockGameDbContext.Object, _mockGameConfig.Object, _mockPlayerManager.Object);
        }

        [Test]
        public async Task BuyTicketsAsync_ShouldThrowIfPlayerNotFound()
        {
            // Arrange
            int playerId = 1;
            Guid gameId = Guid.NewGuid();
            int requestedTickets = 2;

            _mockGameDbContext.Setup(db => db.Players.FindAsync(playerId))
                              .ReturnsAsync((Player)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _ticketManager.BuyTicketsAsync(playerId, gameId, requestedTickets));

            Assert.That(ex.Message, Is.EqualTo("Player not found."));
        }

        [Test]
        public async Task BuyTicketsAsync_ShouldThrowIfPlayerCannotAffordTicket()
        {
            // Arrange
            var player = new Player { Id = 1, PlayerBalance = _mockGameConfig.Object.Value.TicketPrice - 1m };
            Guid gameId = Guid.NewGuid();
            int requestedTickets = 2;

            _mockGameDbContext.Setup(db => db.Players.FindAsync(player.Id))
                              .ReturnsAsync(player);

            // Act & Assert
            Assert.ThrowsAsync<InsufficientFundsException>(async () =>
                await _ticketManager.BuyTicketsAsync(player.Id, gameId, requestedTickets));
        }

        [Test]
        public async Task BuyTicketsAsync_ShouldLimitTicketsToAffordableAmount()
        {
            // Arrange
            var player = new Player { Id = 1, PlayerBalance = 25m };
            Guid gameId = Guid.NewGuid();
            int requestedTickets = 20;

            _mockGameDbContext.Setup(db => db.Players.FindAsync(player.Id))
                              .ReturnsAsync(player);

            // Act
            var tickets = await _ticketManager.BuyTicketsAsync(player.Id, gameId, requestedTickets);

            // Assert
            Assert.That(tickets.Count, Is.EqualTo(2));
        }
    }
}
