using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;

namespace MyUltimaServer.Network
{
    public class Listener
    {
        private readonly Socket m_ListenerSocket;
        private readonly int m_Port = 2593;

        public Listener()
        {
            m_ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Unspecified);
        }

        public void StartListening()
        {
            try
            {
                Console.WriteLine("Listening started port: {0} ",m_Port);
                m_ListenerSocket.Bind(new IPEndPoint(IPAddress.Any, m_Port));
                m_ListenerSocket.Listen(10);
                m_ListenerSocket.BeginAccept(AcceptCallback, m_ListenerSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket acceptedSocket = m_ListenerSocket.EndAccept(ar);
                NetworkState.AddClient(acceptedSocket);
                Console.WriteLine("Accepted Client: {0}", acceptedSocket.LocalEndPoint.ToString());
                m_ListenerSocket.BeginAccept(AcceptCallback, m_ListenerSocket);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

}
