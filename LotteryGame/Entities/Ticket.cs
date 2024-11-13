using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public int PlayerId { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
