using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Exceptions
{
    /// <summary>
    /// Exception Used Declare Insufficient Funds
    /// </summary>
    public class InsufficientFundsException : Exception
    {
        public decimal RequiredAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientFundsException(decimal requiredAmount, decimal currentBalance)
            : base($"Insufficient funds. Required: {requiredAmount:C}, Current Balance: {currentBalance:C}")
        {
            RequiredAmount = requiredAmount;
            CurrentBalance = currentBalance;
        }
    }
}
