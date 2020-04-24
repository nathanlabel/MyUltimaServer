using System;
using System.Collections.Generic;
using System.Text;

namespace MyUltimaServer.Network
{
    public static class ByteConverter
    {
        public static int[] ReadToInt(int offset, int count, byte[] payload)
        {
            int[] returnVal = new int[count];
            for (int x = offset; x < offset + count; x++)
                returnVal[x - offset] = payload[x];
            return returnVal;

        }
        public static String ReadToString(int offset, int count, byte[] payload)
        {
            byte[] stringBytes = new byte[count];
            Array.Clear(stringBytes, 0, 30);
            Buffer.BlockCopy(payload, offset, stringBytes, 0, count);

            return Encoding.ASCII.GetString(stringBytes);
        }
        public static short ReadShort(int offset, byte[]payload)
        {
            return BitConverter.ToInt16(payload, offset);
        }
    }
}
