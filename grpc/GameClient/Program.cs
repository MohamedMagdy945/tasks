using GameServer.Protos;
using Grpc.Core;
using Grpc.Net.Client;

namespace GameClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Game.GameClient(channel);

            using var call = client.JoinGame();

            var sendTask = Task.Run(async () =>
            {
                for (int i = 1; i <= 5; i++)
                {
                    await call.RequestStream.WriteAsync(new PlayerAction
                    {
                        PlayerName = "Ahmed",
                        Answer = $"Answer {i}"
                    });

                    await Task.Delay(1000);
                }

                await call.RequestStream.CompleteAsync();
            });

            var readTask = Task.Run(async () =>
            {
                await foreach (var msg in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"SERVER: {msg.CurrentQuestion} | {msg.RemainingSeconds}");
                }
            });

            await Task.WhenAll(sendTask, readTask);
        }
    }
}
