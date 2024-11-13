using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Models
{
    public class GameConfig
    {
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int MinTickets { get; set; }
        public int MaxTickets { get; set; }
        public decimal TicketPrice { get; set; }
        public decimal DefaultPlayerBalance { get; set; }
    }
}
