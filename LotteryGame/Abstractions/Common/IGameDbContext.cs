using LotteryGame.Entities;
using Microsoft.EntityFrameworkCore;

namespace LotteryGame.Abstractions.Common
{
    public interface IGameDbContext
    {
        DbSet<Ticket> Tickets { get; }
        DbSet<GameInstance> GameInstances { get; }
        DbSet<Player> Players { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
