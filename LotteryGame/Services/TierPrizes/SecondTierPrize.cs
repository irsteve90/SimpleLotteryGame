using LotteryGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Services.TierPrizes
{
    public class SecondTierPrize : TierPrizeBase
    {
        public SecondTierPrize() : base("Second Prize") { }

        /// <summary>
        /// Share Percentage 30%
        /// </summary>
        public override decimal SharePercentage => 30;

        /// <summary>
        /// Select 10% of the tickets
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        public override List<Ticket> SelectWinners(List<Ticket> tickets) => SelectRandomTickets(tickets, (int)Math.Round(tickets.Count * 0.10));
    }
}
