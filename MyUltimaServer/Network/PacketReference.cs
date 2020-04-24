using MyUltimaServer.Accounting;
using System;
using System.Collections.Generic;
using System.Text;
using MyUltimaServer.Server;
using System.Net;

namespace MyUltimaServer.Network
{
    public enum FeatureFlags:int
    {
        T2AFeatures = 0x01,
        LiveAccount = 0x8000
    }
    public static class PacketReference
    {
        public delegate void PacketHandler(PacketArgs e);

        public static Dictionary<byte, PacketHandler> PacketDictionary { get; set; }
        public static Dictionary<byte, int> PacketSizeDictionary { get; set; }

        static PacketReference()
        {
            PacketDictionary = new Dictionary<byte, PacketHandler>();
            PacketSizeDictionary = new Dictionary<byte, int>();

            // Packet Registration

            RegisterPacket(0xEF, 21, ProcessClientLoginSeed);
            RegisterPacket(0x80, 62, ProcessClientLoginRequest);
            RegisterPacket(0xA0, 3, ProcessSelectServer);
        }

        public static void RegisterPacket(byte command, int size, PacketHandler handler)
        {
            if (!PacketDictionary.ContainsKey(command))
            {
                PacketDictionary.Add(command, handler);
                PacketSizeDictionary.Add(command, size);
            }
            else 
                throw new Exception("This command already exists.");
        }

        public static void ProcessClientLoginSeed(PacketArgs e)
        {
            int[] seed = ByteConverter.ReadToInt(1, 4, e.Payload);
            int[] majorVersion = ByteConverter.ReadToInt(5, 4, e.Payload);
            int[] minorVersion = ByteConverter.ReadToInt(9, 4, e.Payload);
            int[] revisionVersion = ByteConverter.ReadToInt(13, 4, e.Payload);
            int[] prototypeVersion = ByteConverter.ReadToInt(17, 4, e.Payload);

            e.Client.SeedClient(seed, majorVersion, minorVersion, revisionVersion, prototypeVersion);

        }
        public static void ProcessClientLoginRequest(PacketArgs e)
        {
            String username = ByteConverter.ReadToString(1, 30, e.Payload).Trim('\0');
           String password = ByteConverter.ReadToString(31, 30, e.Payload).Trim('\0');
            byte nextLoginKey = e.Payload[61];

            ServerState.Accounts.AuthenticateAccount(username, password, e.Client);
        }
        public static void ProcessSelectServer(PacketArgs e)
        {
            short selectedShard = ByteConverter.ReadShort(1, e.Payload);
            SendConnectToGameServer(e.Client);
        }

        public static void SendConnectToGameServer(Client client)
        {
            Packet packet = new Packet(0x8C, 11);
            packet.Write(IPAddress.Parse("127.0.0.1"));
            packet.Write((short)2593);
            packet.SendData(client);

        }
        public static void SendClientLoginFailure(Client client, byte reason)
        {
            Packet packet = new Packet(0x82, 2);
            packet.Write(reason);
            packet.SendData(client);
        }
        public static void SendClientLoginComplete(Client client)
        {
            Packet packet = new Packet(0x55, 1);
            packet.SendData(client);
        }
        public static void SendGameServerList(Client client, string shardname)
        {
            Packet sendAckPacket = new Packet(0xA8, 46);
            sendAckPacket.Write((short)46); // Length
            sendAckPacket.Write((byte)0x5D);
            sendAckPacket.Write((short)1);

            sendAckPacket.Write((short)0);
            sendAckPacket.WriteString("Nath", 32);
            sendAckPacket.Write((byte)0);
            sendAckPacket.Write((byte)0);
            sendAckPacket.Write(IPAddress.Parse("127.0.0.1"));

            sendAckPacket.SendData(client);
        }
        /*public static void SendEnableLockedClientFeatures(Client client)
        {
            byte[] flags = BitConverter.GetBytes((int)FeatureFlags.LiveAccount + (int)FeatureFlags.T2AFeatures);
            byte[] buff = new byte[5];
            buff[0] = 0xB9;
            Array.Copy(flags, 0, buff, 1, 4);
            client.SendData(buff);
        }*/
    }

    public class PacketArgs
    {
        public byte[] Payload { get; }
        public Client Client { get; }

        public PacketArgs(byte[] payload, Client client) : base()
        {
            Payload = payload;
            Client = client;
        }
    }

    
}
