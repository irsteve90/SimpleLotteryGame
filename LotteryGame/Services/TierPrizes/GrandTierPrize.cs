using LotteryGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Services.TierPrizes
{
    public class GrandTierPrize : TierPrizeBase
    {
        public GrandTierPrize() : base("Grand Prize") { }

        /// <summary>
        /// Share Percentage 10%
        /// </summary>
        public override decimal SharePercentage => 50;

        /// <summary>
        /// Select 1 random winner
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        public override List<Ticket> SelectWinners(List<Ticket> tickets) => SelectRandomTickets(tickets, 1);
    }
}
