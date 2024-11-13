using LotteryGame.Entities;
using LotteryGame.Services.TierPrizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Tests.Common
{
    [TestFixture]
    public class TierPrizeTests
    {
        private List<Ticket> GenerateTickets(int count)
        {
            return Enumerable.Range(1, count).Select(i => new Ticket { Id = new Guid() }).ToList();
        }

        [Test]
        public void GrandTierPrize_SelectsOneWinner()
        {
            // Arrange
            var grandPrize = new GrandTierPrize();
            var tickets = GenerateTickets(10);

            // Act
            var winners = grandPrize.SelectWinners(tickets);

            // Assert
            Assert.That(winners.Count, Is.EqualTo(1), "GrandTierPrize should select exactly one winner.");
        }

        [Test]
        public void SecondTierPrize_SelectsTenPercentOfTickets()
        {
            // Arrange
            var secondPrize = new SecondTierPrize();
            var tickets = GenerateTickets(20);

            // Act
            var winners = secondPrize.SelectWinners(tickets);

            // Assert
            int expectedWinnerCount = (int)Math.Round(tickets.Count * 0.10);
            Assert.That(winners.Count, Is.EqualTo(expectedWinnerCount), "SecondTierPrize should select 10% of the tickets.");
        }

        [Test]
        public void ThirdTierPrize_SelectsTwentyPercentOfTickets()
        {
            // Arrange
            var thirdPrize = new ThirdTierPrize();
            var tickets = GenerateTickets(20);

            // Act
            var winners = thirdPrize.SelectWinners(tickets);

            // Assert
            int expectedWinnerCount = (int)Math.Round(tickets.Count * 0.20);
            Assert.That(winners.Count, Is.EqualTo(expectedWinnerCount), "ThirdTierPrize should select 20% of the tickets.");
        }

        [Test]
        public void SelectWinners_HandlesEmptyTicketList()
        {
            // Arrange
            var grandPrize = new GrandTierPrize();
            var secondPrize = new SecondTierPrize();
            var thirdPrize = new ThirdTierPrize();
            var tickets = new List<Ticket>();

            // Act
            var grandWinners = grandPrize.SelectWinners(tickets);
            var secondWinners = secondPrize.SelectWinners(tickets);
            var thirdWinners = thirdPrize.SelectWinners(tickets);

            // Assert
            Assert.That(grandWinners.Count, Is.EqualTo(0), "GrandTierPrize should return no winners for an empty ticket list.");
            Assert.That(secondWinners.Count, Is.EqualTo(0), "SecondTierPrize should return no winners for an empty ticket list.");
            Assert.That(thirdWinners.Count, Is.EqualTo(0), "ThirdTierPrize should return no winners for an empty ticket list.");
        }
    }
}
