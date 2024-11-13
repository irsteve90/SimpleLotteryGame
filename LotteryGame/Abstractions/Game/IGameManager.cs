using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Abstractions.Game
{
    public interface IGameManager
    {
        Task StartNewGame();
    }
}
