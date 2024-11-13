using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Entities
{
    public class GameInstance
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public decimal HouseRevenue { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
