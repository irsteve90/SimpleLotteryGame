using LotteryGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Services.TierPrizes
{
    public abstract class TierPrizeBase
    {
        public string Name { get; }
        protected TierPrizeBase(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Declare Percentage of Winnings Revenue
        /// </summary>
        public abstract decimal SharePercentage { get; }

        /// <summary>
        /// Select Winners for Tier
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        public abstract List<Ticket> SelectWinners(List<Ticket> tickets);

        /// <summary>
        /// Randomly Select Nth Number of Tickets.
        /// </summary>
        /// <param name="tickets"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected List<Ticket> SelectRandomTickets(List<Ticket> tickets, int count)
        {
            var random = new Random();
            return tickets.OrderBy(x => random.Next()).Take(count).ToList();
        }
    }
}
