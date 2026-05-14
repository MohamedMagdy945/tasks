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
            var cts = context.CancellationToken;

            var readTask = Task.Run(async () =>
            {
                while (await requestStream.MoveNext())
                {
                    var action = requestStream.Current;

                    Console.WriteLine($"{action.PlayerName}: {action.Answer}");
                }
            });

            var remaining = 10;

            while (!cts.IsCancellationRequested && remaining >= 0)
            {
                await responseStream.WriteAsync(new GameEvent
                {
                    CurrentQuestion = "What is 2 + 2 ?",
                    RemainingSeconds = remaining,
                    Leaderboard = "Ahmed:10, Ali:8",
                    Winner = ""
                });

                remaining--;
                await Task.Delay(1000);
            }

            await readTask;
        }
    }
}
