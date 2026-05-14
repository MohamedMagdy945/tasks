using GameServer.Protos;
using GameServer.Services;
using Grpc.Core;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server
            {
                Services = { Game.BindService(new GameService()) },
                Ports = { new ServerPort("localhost", 5000, ServerCredentials.Insecure) }
            };

            server.Start();
        }
    }
}
