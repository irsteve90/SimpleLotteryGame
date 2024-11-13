using LotteryGame.Entities;
using LotteryGame.Services.TierPrizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Tests.UnitTests.Game
{
    /// <summary>
    /// Test Class for Prize, 30% share, 2 winners
    /// </summary>
    public class TestTierPrizeClass : TierPrizeBase
    {
        public TestTierPrizeClass() : base("") { }
        public override decimal SharePercentage => 30;
        public override List<Ticket> SelectWinners(List<Ticket> tickets) => SelectRandomTickets(tickets, 2);
    }

    /// <summary>
    /// Test Class for Prize, full share return, 2 winners
    /// </summary>
    public class TestTierPrizeClassFullReturn : TierPrizeBase
    {
        public TestTierPrizeClassFullReturn() : base("") { }
        public override decimal SharePercentage => 100;
        public override List<Ticket> SelectWinners(List<Ticket> tickets) => SelectRandomTickets(tickets, 2);
    }
}
