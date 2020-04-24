using System;
using System.Collections.Generic;
using System.Text;

namespace MyUltimaServer.Network
{
    public class ByteQueue
    {
        private Queue<byte[]> m_Data;
        private byte[] m_Buffer;
        private int m_Index;
        private Client m_Client;

        public ByteQueue(Client client)
        {
            m_Data = new Queue<byte[]>();
            m_Client = client;
        }

        public void Enqueue(byte[] data)
        {
            byte[] temp = new byte[data.Length];
            Buffer.BlockCopy(data, 0, temp, 0, data.Length);
            m_Data.Enqueue(temp);
        }

        public void ProcessPackets()
        {
            lock (m_Data)
            {
                if (m_Buffer == null && m_Data.Count > 0)
                {
                    byte[] tmp = m_Data.Dequeue();
                     m_Buffer = new byte[tmp.Length];
                    Array.Clear(m_Buffer, 0, m_Buffer.Length);
                    Buffer.BlockCopy(tmp, 0, m_Buffer, 0, tmp.Length);
                }

                if (m_Buffer != null && m_Data.Count > 0)
                {
                    byte[] temp = m_Data.Dequeue();
                    int originalSize = m_Buffer.Length;
                    Array.Resize(ref m_Buffer, m_Buffer.Length + temp.Length);
                    Buffer.BlockCopy(temp, 0, m_Buffer, originalSize, temp.Length);
                }

                if (m_Buffer != null && m_Index < m_Buffer.Length)
                {
                    if (m_Buffer[m_Index] != 0)
                    {
                        int packetSize = PacketReference.PacketSizeDictionary[m_Buffer[m_Index]];
                        byte[] payload = new byte[packetSize];
                        Buffer.BlockCopy(m_Buffer, m_Index, payload, 0, packetSize);
                        PacketReference.PacketDictionary[payload[0]](new PacketArgs(payload, m_Client));
                        m_Index += packetSize;
                    }
                    else
                        m_Index++;

                    if (m_Index >= m_Buffer.Length)
                    {
                        m_Buffer = null;
                        m_Index = 0;
                    }
                }
            }
        }
    }
}
