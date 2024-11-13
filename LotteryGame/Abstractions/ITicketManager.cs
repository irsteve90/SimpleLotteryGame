using LotteryGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Abstractions
{
    public interface ITicketManager
    {
        Task<List<Ticket>> BuyTicketsAsync(int playerId, Guid gameId, int requestedTickets);
        Task<List<Ticket>> GetTicketsForGameAsync(Guid gameId);
    }
}
