using LotteryGame.Entities;
using LotteryGame.Models;

namespace LotteryGame.Abstractions.Game
{
    public interface IGameUI
    {
        public void ShowMessage(string message);
        public void ClearMessages();
        public void ShowNewLine();
        public int RequestTicketsAmount(Player player);
        public void ShowRemainingBalanceMessage(decimal balance);
        public void ShowDrawResultsMessage(DrawResult drawResult);
        public void ShowHouseRevenueMessage(decimal revenue);
        public void ShowsOtherPlayersInGameMessage(int playerCount);
        public void ShowWelcomeMessage(Player player);
        public bool RequestRestartGameMenu();
    }
}
