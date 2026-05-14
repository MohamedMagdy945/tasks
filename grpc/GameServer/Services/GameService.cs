using GameServer.Protos;
using Grpc.Core;

namespace GameServer.Services
{
    public class GameService : Game.GameBase
    {
        public override async Task JoinGame(
        IAsyncStreamReader<PlayerAction> requestStream,
        IServerStreamWriter<GameEvent> responseStream,
    ServerCallContext context)
        {
            var readTask = Task.Run(async () =>
            {
                await foreach (var action in requestStream.ReadAllAsync())
                {
                    Console.WriteLine($"{action.PlayerName}: {action.Answer}");
                }
            });

            var writeTask = Task.Run(async () =>
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(new GameEvent
                    {
                        CurrentQuestion = "What is 2 + 2 ?",
                        RemainingSeconds = 10,
                        Leaderboard = "Ahmed:10, Ali:8",
                        Winner = ""
                    });

                    await Task.Delay(4000);
                }
            });

            await Task.WhenAll(readTask, writeTask);
        }
    }
}
