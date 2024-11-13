using LotteryGame.Services.TierPrizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Models
{
    public class DrawResult
    {
        public Dictionary<TierPrizeBase, TierResult> TierResults { get; set; } = new Dictionary<TierPrizeBase, TierResult>();
        public decimal HouseRevenue { get; set; }
    }
}
