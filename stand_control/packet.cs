

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace com_port
{

    public class Trans_protocol : Packet_struct
    {  

        public byte[]       commun_data_buffer = new byte[10000];
        public byte[]       data_buffer = new byte[10000];


        

        public Trans_protocol()
        {
            Receive_reset();
            header_size = Get_header_size(); // записываем размер хедера
        }
        //================================================================================================
        public void Write_data_tx(dynamic buffer, uint len)
        {
            byte[] buffer2;
            if (buffer.Length > 1) buffer2 = (byte[])buffer;

            data = new byte[len];
            Buffer.BlockCopy(buffer, 0, data, 0, (int)len);
            data_size = len;
            uint packet_size = (uint)header_size + len + Constants.CRC_BYTE_SIZE;
            Set_len_pack( packet_size);
            Set_crc_pack();

        }
        //================================================================================================
        public void Write_data_tx( byte Char)
        {
            data = new byte[1];
            data[0] = Char;
            data_size = 1;

            uint packet_size = (uint)header_size + data_size + Constants.CRC_BYTE_SIZE;
            Set_len_pack(packet_size);
            Set_crc_pack();
        }
        //================================================================================================
        public void Send_protocol_packet()
        {
            uint _len = Get_len_pack();
            uint contr_len = 0;

            byte[] buffer = new byte[Get_len_pack()];

            contr_len = (uint)Get_commun_buffer(ref buffer);

            if (contr_len != _len)
                while (1 == 1)
                {
           
                }
            try
            {
                serial.Myserial.Write(buffer, 0, (int)_len);
            }
            catch (System.IO.IOException)
            {

            }
        }
        //================================================================================================

        /*
        *возвращает -1 если была ошибка приема мультипакет 
        * -2 если прнимается мультипакет
        * >0 если данные готовы для приема
        */
        //================================================================================================
        public int Multipack_receive()
        {
            int result = 0;

            if (header.packet_number == 0) // если пришел не мультипакет
            {
                uint data_size = Get_len_pack() - (uint)header_size - Constants.CRC_BYTE_SIZE;
                Array.Copy(data, 0, data_buffer, 0, data_size);
                Protocol._pack_handler(data);
                return (int)data_size;
            }
            else // если принимаем мультипакет
            {
                int pack_number = header.packet_number & ~0x80;

                if (pack_number != flag_multipack) // если принят пакет с не тем номером
                {
                    byte_counter = 0;
                    flag_multipack = 1;
                    return -1;
                }

                uint data_size = Get_len_pack() - (uint)header_size - Constants.CRC_BYTE_SIZE;
                Array.Copy(data, 0, commun_data_buffer, byte_counter, data_size);
                byte_counter += data_size;

                if ((header.packet_number & 0x80) > 0) // если будет передаваться еще пакет
                {
                    flag_multipack++;
                    return -2;
                }
                else  // если пакет последний  то можно вытаскивать изнего данные
                {
                    flag_multipack = 1;
                    result = (int)byte_counter;
                    byte_counter = 0;
                    Protocol._pack_handler(commun_data_buffer);
                    return result;
                }
            }
        }
    }
}
