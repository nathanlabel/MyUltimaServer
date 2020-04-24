using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MyUltimaServer.Network
{
    public static class NetworkState
    {
        public static List<Client> Clients = new List<Client>();
        public static void AddClient(Socket socket)
        {
            Clients.Add(new Client(socket));
        }
        public static void RemoveClient(Client client)
        {
            Clients.Remove(client);
        }
    }
}
