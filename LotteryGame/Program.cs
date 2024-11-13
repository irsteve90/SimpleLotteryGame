using LotteryGame.Abstractions.Common;
using LotteryGame.Abstractions;
using LotteryGame.Models;
using LotteryGame.Repositories;
using LotteryGame.Services.Game;
using LotteryGame.Services.TierPrizes;
using LotteryGame.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using LotteryGame.Abstractions.Game;

namespace LotteryGame
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            // Register Db Context and dependencies
            services.AddDbContext<GameDbContext>(options =>
                options.UseInMemoryDatabase(nameof(GameDbContext)))
            .AddScoped<IGameDbContext>(provider => provider.GetRequiredService<GameDbContext>());

            // Register services and dependencies
            services.Configure<GameConfig>(configuration.GetSection("GameConfig"));
            services.AddScoped<ITicketManager, TicketManager>();
            services.AddScoped<IGameManager, GameManager>();
            services.AddScoped<IGameUI, ConsoleGameUI>();
            services.AddScoped<IPlayerManager, PlayerManager>();

            // Register all TierPrizeBase types as a collection
            services.AddSingleton<TierPrizeBase, GrandTierPrize>();
            services.AddSingleton<TierPrizeBase, SecondTierPrize>();
            services.AddSingleton<TierPrizeBase, ThirdTierPrize>();

            // Set the culture to en-US
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var serviceProvider = services.BuildServiceProvider();

            await StartGame(serviceProvider);
        }

        public static async Task StartGame(ServiceProvider serviceProvider)
        {
            try
            {
                // Get Game Coordinator Service and Start Game
                var gameManager = serviceProvider.GetRequiredService<IGameManager>();

                await gameManager.StartNewGame();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred running the game: {ex.Message}");
            }
        }
    }
}
