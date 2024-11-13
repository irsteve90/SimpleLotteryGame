using LotteryGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Models
{
    public class TierResult
    {
        public List<Ticket> TicketsWon { get; set; } = new List<Ticket> { };
        public decimal RevenuePerTicket { get; set; }
    }
}
