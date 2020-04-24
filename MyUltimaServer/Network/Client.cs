using MyUltimaServer.Accounting;
using MyUltimaServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyUltimaServer.Network
{
    public class Client
    {
        private byte[] m_Buffer;
        private int[] m_Seed;
        private int[] m_MajorVersion;
        private int[] m_MinorVersion;
        private int[] m_RevisionVersion;
        private int[] m_PrototypeVersion;
        private bool m_Seeded; // Has the client been seeded?
        private Account m_Account; // The account registered to this particular client
        private NetworkStream networkStream;

        public ByteQueue ReceivedData { get; set; }
        public Socket ClientSocket { get; set; }
        public int ID { get; set; }
        public String Seed
        {
            get
            {
                return m_Seed[0].ToString() + "." + m_Seed[1] + "." + m_Seed[2] + "." + m_Seed[3];
            }
        }
        public String MajorVersion
        {
            get
            {
                return m_MajorVersion[0].ToString() + "." + m_MajorVersion[1] + "." + m_MajorVersion[2] + "." + m_MajorVersion[3];
            }
        }
        public String MinorVersion
        {
            get
            {
                return m_MinorVersion[0].ToString() + "." + m_MinorVersion[1] + "." + m_MinorVersion[2] + "." + m_MinorVersion[3];
            }
        }
        public String RevisionVersion
        {
            get
            {
                return m_RevisionVersion[0].ToString() + "." + m_RevisionVersion[1] + "." + m_RevisionVersion[2] + "." + m_RevisionVersion[3];
            }
        }
        public String PrototypeVersion
        {
            get
            {
                return m_PrototypeVersion[0].ToString() + "." + m_PrototypeVersion[1] + "." + m_PrototypeVersion[2] + "." + m_PrototypeVersion[3];
            }
        }
        public bool IsSeeded
        {
            get { return m_Seeded; }
        }
        public bool Authenticated
        {
            get
            {
                if (m_Account == null)
                    return false;
                else
                    return true;
            }
        }

        public Client(Socket socket)
        {
            ID = NetworkState.Clients.Count;
            ClientSocket = socket;
            networkStream = new NetworkStream(socket);
            m_Seeded = false;
            ReceivedData = new ByteQueue(this);
            StartReceiving();
        }

        public void SeedClient(int[] seed, int[] major, int[] minor, int[] revisiion, int[] prototype)
        {
            if (!m_Seeded)
            {
                m_Seed = seed;
                m_MajorVersion = major;
                m_MinorVersion = minor;
                m_RevisionVersion = revisiion;
                m_PrototypeVersion = prototype;
                m_Seeded = true;
#if DEBUG
                Console.WriteLine("Client Seeded: {0} ", Seed);
                Console.WriteLine("MajorVersion: {0}", MajorVersion);
                Console.WriteLine("MinorVersion: {0}", MinorVersion);
                Console.WriteLine("RevisionVersion: {0}", RevisionVersion);
                Console.WriteLine("ProtoTypeVersion: {0}", PrototypeVersion);
#endif
            }
            else throw new Exception("Client: " + Seed + " has already been seeded.");
        }
        public void AssignAccount(Account account)
        {
            if (!Authenticated)
            {
                m_Account = account;
            }
            else
                throw new Exception("The client / account has already been authenticated.");

        }
        private void StartReceiving()
        {
            m_Buffer = new byte[256];
            ClientSocket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            int bytesRead = ClientSocket.EndReceive(ar);
            if (bytesRead > 0)
                ReceivedData.Enqueue(m_Buffer);
            StartReceiving();
        }
        private void Disconnect()
        {
            ClientSocket.Disconnect(true);
            NetworkState.RemoveClient(this);
        }
        
        public void WriteToStream(byte[] payload)
        {
            networkStream.BeginWrite(payload, 0, payload.Length, null, networkStream);
        }
    }
}
