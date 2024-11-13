using LotteryGame.Abstractions.Common;
using LotteryGame.Entities;
using Microsoft.EntityFrameworkCore;

namespace LotteryGame.Repositories
{
    public class GameDbContext : DbContext, IGameDbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<GameInstance> GameInstances => Set<GameInstance>();
        public DbSet<Player> Players => Set<Player>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
