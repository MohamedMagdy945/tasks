using GameServer.Protos;
using Grpc.Core;
using Grpc.Net.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // مهم: نفس port بتاع السيرفر
        using var channel = GrpcChannel.ForAddress("http://localhost:5000");

        var client = new Game.GameClient(channel);

        using var call = client.JoinGame();
        // Task لإرسال البيانات (Client Stream)
        var sendTask = Task.Run(async () =>
        {
            while (true)
            {
                var input = Console.ReadLine();

                await call.RequestStream.WriteAsync(new PlayerAction
                {
                    PlayerName = "Ahmed",
                    Answer = input
                });
            }
        });

        while (await call.ResponseStream.MoveNext())
        {
            var gameEvent = call.ResponseStream.Current;

            Console.WriteLine("Question: " + gameEvent.CurrentQuestion);
            Console.WriteLine("Time: " + gameEvent.RemainingSeconds);
            Console.WriteLine("Leaderboard: " + gameEvent.Leaderboard);
            Console.WriteLine("Winner: " + gameEvent.Winner);
            Console.WriteLine("----------------------------------");
        }

        await sendTask;
    }
}