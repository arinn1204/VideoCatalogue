using GrainsInterfaces.Models.CodecParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Grains.CodecParser
{
    public class Parser
    {
        public FileInformation GetInformation(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var id = GetId(stream);

            return new FileInformation();
        }

        private uint GetId(FileStream stream)
        {
            var b = (byte)stream.ReadByte();
            if (b == 0xEC || b == 0xBF)
            {
                return b;
            }

            var convertedByte = GetUInt32(b, stream);

            switch(convertedByte)
            {
                case 0x100:
                    return convertedByte;
                default:
                    return 0;
            };
        }

        private uint GetUInt32(byte firstByte, FileStream stream)
        {
            var float32 = new Float32()
            {
                B4 = firstByte,
                B3 = (byte)stream.ReadByte(),
                B2 = (byte)stream.ReadByte(),
                B1 = (byte)stream.ReadByte(),
            };

            return float32.UnsignedData;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float32
    {
        [FieldOffset(0)]
        public byte B1;

        [FieldOffset(1)]
        public byte B2;

        [FieldOffset(2)]
        public byte B3;

        [FieldOffset(3)]
        public byte B4;

        [FieldOffset(0)]
        public Single Data;

        [FieldOffset(0)]
        public UInt32 UnsignedData;
    }
}
