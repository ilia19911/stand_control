using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace com_port
{
    public class Constants
    {
        public const uint LEN_BYTE_SIZE     = 2;
        public const uint VERSION_BYTE_SIZE = 2;
        public const uint CRC_BYTE_SIZE     = 4;
        public static readonly byte[] id = new byte[] { 0xB0, 0x3B, 0x67 };
    }

    public class header_struct
    {

       public byte[]  id              = new byte[3];
       public byte    type;             //
       public byte[]  len             = new byte[2];
       public byte[]  alen            = new byte[2];
       public byte    packet_number;    //
       public byte[]  version         = new byte[2];
    }
    public class Header_structure
    {
        public header_struct header = new header_struct();
        

        public Header_structure()
        {

         }

        public uint header_size;

        public enum Status
        {
            id,
            type,
            len,
            alen,
            number,
            version,
            data,
            crc
        };
        //================================================================================
        public int Byte_to_int(byte byteL, byte byteH)
        {
            int result = 0;
            result = byteL | (byteH << 8);
            return result;

        }
        //================================================================================
        public uint Int_to_byte(uint data, ref byte byteL, ref byte byteH)
        {
            byteL = (byte)(data & 0xFF);
            byteH = (byte)((data & 0xFF00) >> 8);
            return 2;
        }
        //================================================================================
        public uint Get_header_size()
        {
            int result = 0;

            result += header.id.Length;
            //result += packet.header.type.;
            result++;
            result += header.len.Length;
            result += header.alen.Length;
            //result += packet.header.packet_number.Length;
            result++;
            result += header.version.Length;

            return (uint)result;
        }

        //================================================================================
        public void Construct_header(Packet_type _type)
        {
            Buffer.BlockCopy(Constants.id, 0, header.id, 0, header.id.Length);

            header.type = (byte)_type;

            header.packet_number = 0;
        }
        //================================================================================================
        public uint Get_len_pack()
        {
            uint _len;
            _len = (uint)Byte_to_int(header.len[0], header.len[1]);
            return _len;
        }
        //================================================================================================
        public uint Get_alen_pack()
        {
            uint _len;
            _len = (uint)Byte_to_int(header.alen[0], header.alen[1]);
            return _len;
        }
        //================================================================================
        public void Set_len_pack(uint _len)
        {
            Int_to_byte(_len, ref header.len[0], ref header.len[1]); // set len

            _len = ~_len;

            Int_to_byte(_len, ref header.alen[0], ref header.alen[1]); // set alen
        }
        //============================================================================================================
        public int _control_lengths()
        {
            uint _len = Get_len_pack();
            uint _alen = Get_alen_pack();

            _alen = 65535 - _alen;
            if (_len != _alen)
            {
                return 0;
            }

            if (_len > 2000)
            {
                return -1;
            }
            //structure.size_of_data = _len - (packet_struct.size_of_crc + (uint)header_size);
            //structure.data = default;
            //structure.data = new byte[structure.size_of_data +1]; // чтобы небыло ошибки, создаем хотя бы один элемент массива
            return 1;
        }
        //================================================================================================

    }
}
