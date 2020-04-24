using MyUltimaServer.Accounting;
using MyUltimaServer.Data;
using MyUltimaServer.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace MyUltimaServer.Server
{
    public static class ServerState
    {
        public static Accounts Accounts = new Accounts();
        public static string ShardName { get; set; }
        public static Time ServerTime { get; private set; }
        private static bool Exit = false;

        public static void LoadAllData()
        {

            Console.WriteLine("Loading Server Data {0}", DataPaths.ServerSettingsFullPath);
            if (Deserialize(DataPaths.ServerSettingsFullPath))
                Console.WriteLine("Successfully loaded Server Data,");
            else
                Console.WriteLine("Failed to load server data");

            Console.WriteLine("Loading Account Data: {0}", DataPaths.AccountDataFullPath);
            if (Accounts.Deserialize(DataPaths.AccountDataFullPath))
                Console.WriteLine("Accounts loaded Successfully");
            else
                Console.WriteLine("No Account Data loaded.");
        }

        public static void StartServerProcessing()
        {
            Thread networkThread = new Thread(new ThreadStart(StartNetworkProcessing));
            networkThread.Start();

            ServerTime = new Time();

            while (!Exit)
            {
                Thread.Sleep(5000);
                Console.WriteLine("Main Server Thread executing...");
            }
        }

        public static void StartNetworkProcessing()
        {
            Listener listener = new Listener();
            listener.StartListening();

            while (true)
            {
                for (int x = 0; x < NetworkState.Clients.Count; x++)
                    NetworkState.Clients[x].ReceivedData.ProcessPackets();
            }
        }

        public static bool Deserialize(string datapath)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                FileStream stream = new FileStream(datapath, FileMode.Open, FileAccess.Read);
                xmldoc.Load(stream);
                XmlNode list = xmldoc.GetElementsByTagName("Server")[0];

                foreach (XmlNode node in list.ChildNodes)
                {
                    if (node.Name == "ShardName")
                        ShardName = node.InnerText;
                }
                return true;
            }
            catch (Exception e)
            {
                ShardName = "Default Shard";
                Console.WriteLine(e);
                return false;
            }
        }

        
    }
}
