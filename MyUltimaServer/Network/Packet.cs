using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MyUltimaServer.Network
{
    class Packet
    {
        private byte[] buffer;
        private int index;
        public Packet(byte cmd, int size)
        {
            buffer = new byte[size];
            buffer[0] = cmd;
            index = 1;
        }

        public void Write(int val)
        {
            byte[] intBytes = BitConverter.GetBytes(val);
            Buffer.BlockCopy(intBytes, 0, buffer, index, intBytes.Length);
            index += intBytes.Length;
        }
        public void Write(short val)
        {
            byte[] shortBytes = BitConverter.GetBytes(val);
            Array.Reverse(shortBytes);
            Buffer.BlockCopy(shortBytes, 0, buffer, index, shortBytes.Length);
            index += shortBytes.Length;
        }
        public void Write(byte b)
        {
            buffer[index] = b;
            index++;
        }
        public void Write(IPAddress ipadddr)
        {
            byte[] addressBytes = ipadddr.GetAddressBytes();
            Array.Reverse(addressBytes);
            Buffer.BlockCopy(addressBytes, 0, buffer, index, addressBytes.Length);
        }
        public void WriteString(string val, int length)
        {
            byte[] stringBytes = Encoding.ASCII.GetBytes(val);
            byte[] buff = new byte[length];
            Buffer.BlockCopy(stringBytes, 0, buff, 0, stringBytes.Length);
            Buffer.BlockCopy(buff, 0, buffer, index, buff.Length);
            index += buff.Length;
        }

        public void SendData(Client client)
        {
            client.WriteToStream(buffer);
        }
    }
}
