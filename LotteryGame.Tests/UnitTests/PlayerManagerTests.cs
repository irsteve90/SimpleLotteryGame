using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LotteryGame.Abstractions.Common;
using LotteryGame.Services;
using LotteryGame.Entities;
using MockQueryable.Moq;
using LotteryGame.Models;
using Microsoft.Extensions.Options;
using LotteryGame.Abstractions;


namespace LotteryGame.Tests.UnitTests
{
    [TestFixture]
    public class PlayerManagerTests
    {
        private Mock<IGameDbContext> _mockGameDbContext;
        private Mock<IOptions<GameConfig>> _mockGameConfig;
        private PlayerManager _playerManager;

        [SetUp]
        public void SetUp()
        {
            _mockGameDbContext = new Mock<IGameDbContext>();

            var mockPlayers = new List<Player>().AsQueryable().BuildMockDbSet();
            _mockGameDbContext.Setup(db => db.Players).Returns(mockPlayers.Object);

            _mockGameConfig = new Mock<IOptions<GameConfig>>();
            _mockGameConfig.Setup(x => x.Value).Returns(new GameConfig
            {
                DefaultPlayerBalance = 10.00m
            });

            _playerManager = new PlayerManager(_mockGameDbContext.Object, _mockGameConfig.Object);
        }

        [Test]
        public async Task CreatePlayerAsync_ShouldCreateSinglePlayer()
        {
            // Act
            var player = await _playerManager.CreatePlayerAsync();

            // Assert
            Assert.IsNotNull(player);
            Assert.That(player.PlayerBalance, Is.EqualTo(_mockGameConfig.Object.Value.DefaultPlayerBalance));
        }

        [Test]
        public async Task CreatePlayersAsync_ShouldCreateMultiplePlayers()
        {
            // Act
            var players = await _playerManager.CreatePlayersAsync(3);

            // Assert
            Assert.That(players.Count, Is.EqualTo(3));
            Assert.That(players.First().PlayerBalance, Is.EqualTo(_mockGameConfig.Object.Value.DefaultPlayerBalance));
            Assert.That(players.Last().PlayerBalance, Is.EqualTo(_mockGameConfig.Object.Value.DefaultPlayerBalance));
        }

        [Test]
        public void CreatePlayersAsync_ShouldThrowArgumentException_WhenPlayerCountIsZeroOrLess()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _playerManager.CreatePlayersAsync(0));
            Assert.That(ex.Message, Is.EqualTo("The number of players must be greater than zero. (Parameter 'playerCount')"));
        }
    }
}
