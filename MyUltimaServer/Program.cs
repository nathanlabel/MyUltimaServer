using System;
using MyUltimaServer.Network;
using MyUltimaServer.Data;
using MyUltimaServer.Server;

namespace MyUltimaServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerState.LoadAllData();

            ServerState.StartServerProcessing();
        }

    }
}
