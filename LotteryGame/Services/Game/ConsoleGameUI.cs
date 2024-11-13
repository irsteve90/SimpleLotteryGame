using Ardalis.GuardClauses;
using LotteryGame.Abstractions.Game;
using LotteryGame.Entities;
using LotteryGame.Models;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace LotteryGame.Services.Game
{
    /// <summary>
    /// Game UI Service, Handles the console presentation/input layer
    /// </summary>
    public class ConsoleGameUI : IGameUI
    {
        private readonly GameConfig _gameConfig;

        public ConsoleGameUI(IOptions<GameConfig> gameConfig)
        {
            _gameConfig = Guard.Against.Null(gameConfig?.Value);
        }

        public void ShowMessage(string message) => Console.WriteLine(message);
        public void ClearMessages() => Console.Clear();
        public void ShowNewLine() => ShowMessage(String.Empty);
        public int RequestTicketsAmount(Player player)
        {
            ShowMessage($"How many tickets do you want to buy, Player {player.Id}?");
            return RequestValidTicketNumber();
        }
        public void ShowRemainingBalanceMessage(decimal balance) => ShowMessage($"Remaining Balance: {balance:C2}");
        
        public void ShowDrawResultsMessage(DrawResult drawResult)
        {
            ShowMessage($"Ticket Draw Results:");
            ShowNewLine();
            foreach (var tierResult in drawResult.TierResults)
            {
                if (tierResult.Value.TicketsWon.Count() == 0) continue;

                StringBuilder resultStringBuilder = new StringBuilder();
                resultStringBuilder.Append($"* {tierResult.Key.Name}: ");
                if (tierResult.Value.TicketsWon.Count == 1)
                {
                    resultStringBuilder.Append($"Player {tierResult.Value.TicketsWon.First().PlayerId} wins {tierResult.Value.RevenuePerTicket:C2}!");
                }
                else
                {
                    resultStringBuilder.Append("Players ");
                    resultStringBuilder.Append(String.Join(", ", tierResult.Value.TicketsWon.Select(x => x.PlayerId).Distinct()));
                    resultStringBuilder.Append($" win {tierResult.Value.RevenuePerTicket:C2} each!");
                    resultStringBuilder.Append(", per winning ticket.");
                }
                ShowMessage(resultStringBuilder.ToString());
            }
            ShowNewLine();
            ShowMessage("Congratulations to the winners!");
        }
        public void ShowHouseRevenueMessage(decimal revenue) => ShowMessage($"House Revenue: {revenue.ToString("C2")}");
        public void ShowsOtherPlayersInGameMessage(int playerCount) => ShowMessage($"{playerCount} other CPU players also have purchased tickets.");
        public void ShowWelcomeMessage(Player player) 
        {
            ShowMessage($"Welcome to the Bede Lottery, Player {player.Id}!");
            ShowMessage($"* Your digital balance: {player.PlayerBalance.ToString("C2")}");
            ShowMessage($"* Ticket Price: {_gameConfig.TicketPrice.ToString("C2")} each");
        }
        public bool RequestRestartGameMenu()
        {
            ShowMessage("Thank you for playing, Would you like to:");
            ShowMessage("1. Replay");
            ShowMessage("2. Quit");

            int input = RequestValidGameOptions();
            return input == 1;
        }

        private int RequestValidGameOptions()
        {
            int input = RequestValidInt();
            int[] numberOptions = new int[] { 1, 2 };
            if (!numberOptions.Contains(input))
            {
                ShowMessage($"Number must be {String.Join(" or ", numberOptions)}.");
                RequestValidGameOptions();
            }

            return input;
        }
        private int RequestValidInt()
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out var number))
                return number;
            else
            {
                ShowMessage($"{input} is not a valid number.");
                return RequestValidInt();
            }
        }
        private int RequestValidTicketNumber()
        {
            var requestedTickets = RequestValidInt();
            if (requestedTickets < _gameConfig.MinTickets || requestedTickets > _gameConfig.MaxTickets)
            {
                ShowMessage($"Requested tickets must be between {_gameConfig.MinTickets} and {_gameConfig.MaxTickets}.");
                return RequestValidTicketNumber();
            }

            return requestedTickets;
        }
    }
}
