using LotteryGame.Entities;
using LotteryGame.Models;
using LotteryGame.Services.Game;
using Microsoft.Extensions.Options;
using Moq;

namespace LotteryGame.Tests.UnitTests.Game
{
    [TestFixture]
    public class GameUITests
    {
        private ConsoleGameUI _gameUI;
        private Mock<IOptions<GameConfig>> _mockGameConfig;

        [SetUp]
        public void SetUp()
        {
            // Setup GameConfig with mock values
            _mockGameConfig = new Mock<IOptions<GameConfig>>();
            _mockGameConfig.Setup(config => config.Value).Returns(new GameConfig
            {
                MinTickets = 1,
                MaxTickets = 10,
                TicketPrice = 1.00m
            });

            _gameUI = new ConsoleGameUI(_mockGameConfig.Object);
        }

        [Test]
        public void RequestTicketsAmount_ShouldAskForTicketCount()
        {
            // Arrange
            var player = new Player { Id = 1 };

            // Redirect Console.WriteLine to capture output
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Console.SetIn(new StringReader("5"));
                // Act
                _gameUI.RequestTicketsAmount(player);

                // Assert
                var output = sw.ToString();
                StringAssert.Contains($"How many tickets do you want to buy, Player {player.Id}?", output);
            }
        }

        [Test]
        public void RequestTicketsAmount_ShouldReturnValidTicketNumber()
        {
            // Arrange
            var player = new Player { Id = 1 };

            // Mock Console input
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Console.SetIn(new StringReader(_mockGameConfig.Object.Value.MaxTickets.ToString()));

                // Act
                var tickets = _gameUI.RequestTicketsAmount(player);

                // Assert
                Assert.That(tickets, Is.EqualTo(_mockGameConfig.Object.Value.MaxTickets));
            }
        }

        [Test]
        public void RequestTicketsAmount_ShouldHandleInvalidInput()
        {
            // Arrange
            var player = new Player { Id = 1 };

            // Mock Console input with invalid number first, then valid number
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Console.SetIn(new StringReader("Not Number"));

                // Act
                var tickets = _gameUI.RequestTicketsAmount(player);

                // Assert
                Assert.That(tickets, Is.EqualTo(5));
                var output = sw.ToString();
                StringAssert.Contains("invalid is not a valid number.", output);
            }
        }

        [Test]
        public void RequestTicketsAmount_ShouldHandleOutOfRangeTickets()
        {
            // Arrange
            var player = new Player { Id = 1 };

            // Mock Console input with invalid number (out of range)
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Console.SetIn(new StringReader((_mockGameConfig.Object.Value.MaxTickets + 1).ToString()));

                // Act
                var tickets = _gameUI.RequestTicketsAmount(player);

                // Assert
                Assert.That(tickets, Is.EqualTo(5));
                var output = sw.ToString();
                StringAssert.Contains("Requested tickets must be between 1 and 10.", output);
            }
        }
    }
}
