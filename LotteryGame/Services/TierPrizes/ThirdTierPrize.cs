using LotteryGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Services.TierPrizes
{
    public class ThirdTierPrize : TierPrizeBase
    {
        public ThirdTierPrize() : base("Third Prize") { }

        /// <summary>
        /// Share Percentage 10%
        /// </summary>
        public override decimal SharePercentage => 10;

        /// <summary>
        /// Select 20% of the tickets
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        public override List<Ticket> SelectWinners(List<Ticket> tickets) => SelectRandomTickets(tickets, (int)Math.Round(tickets.Count * 0.20));
    }
}
