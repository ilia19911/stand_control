using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com_port
{
    public class Crc
    {
        static uint[] crc32_table = new uint[1024];
        static uint[] crc32r_table = new uint[1024];
        const uint CRC32_POLY = 0x04C11DB7;
        const uint CRC32_POLY_R = 0xEDB88320;

        public Crc()
        {
            int i, j;
            uint c, cr;
            for (i = 0; i < 256; ++i)
            {
                cr = (uint)i;
                c = (uint)(i << 24);
                for (j = 8; j > 0; --j)
                {
                    c = ((c & 0x80000000) > 0) ? (c << 1) ^ CRC32_POLY : (c << 1);
                    cr = ((cr & 0x00000001) > 0) ? (cr >> 1) ^ CRC32_POLY_R : (cr >> 1);
                }
                crc32_table[i] = c;
                crc32r_table[i] = cr;
                //dprintf("crc32r_table[%u] = %X ", i, crc32r_table[i]);
            }
        }
        //================================================================================================
        public uint Crc32_sftwr(uint init_crc, dynamic buf, int len)
        {
            byte[] buf2 = (byte[])buf;
            int v;
            uint crc;
            crc = ~init_crc;
            for (uint i = 0; i < len; i++)
            {
                v = buf2[i];
                crc = (crc >> 8) ^ crc32r_table[(crc ^ (v)) & 0xff];
            }
            return ~crc;
        }

    }
}
