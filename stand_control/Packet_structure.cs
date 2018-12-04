using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace com_port
{
    public class Packet_struct : Header_structure
    {
        int counter;
        int flag;
        public int flag_multipack;
        public uint byte_counter;

        public byte[] data;
        public byte[] crc = new byte[Constants.CRC_BYTE_SIZE];
        public Crc myCrc = new Crc();

        public uint data_size;

        public enum PACKET_STATUS
        {
            NO_PACKETS,
            MULTI_PACK
        };
        //================================================================================
        public void Set_crc_pack()
        {
            uint _crc = 0;
            uint len = Get_len_pack() - Constants.CRC_BYTE_SIZE;
            byte[] buffer = new byte[Get_len_pack()];

            Get_commun_buffer(ref buffer);

            _crc = myCrc.Crc32_sftwr(0, buffer, (int)len);

            crc[0] = Convert.ToByte(_crc & 0xFF);
            crc[1] = Convert.ToByte((_crc & 0xFF00) >> 8);
            crc[2] = Convert.ToByte((_crc & 0xFF0000) >> 16);
            crc[3] = Convert.ToByte((_crc & 0xFF000000) >> 24);
        }
        //================================================================================

        public uint _control_crc()
        {
            uint actual_crc = 0;
            uint calc_crc = 0;
            byte[] temp_buffer = new byte[Get_len_pack()]; // массив для записи восстановленных данных из структуры
            Get_commun_buffer(ref temp_buffer);

            calc_crc = myCrc.Crc32_sftwr(0, temp_buffer, (int)Get_len_pack() - (int)Constants.CRC_BYTE_SIZE);

            actual_crc = (uint)(((crc[3] & 0xff) << 24) | ((crc[2] & 0xff) << 16) | ((crc[1] & 0xff) << 8) | ((crc[0] & 0xff)));

            if (calc_crc != actual_crc)
            {
                return 0;
                // return 1;
            }
            return 1;
        }
        //============================================================================================================
        // функция для получения общего массива принятых данных
        public int Get_commun_buffer(ref byte[] buffer) //возвращает количество записанных данных
        {
          //  GetBytes(ref header);
            int _counter = 0;

            Buffer.BlockCopy(header.id, 0, buffer, _counter, header.id.Length);
            _counter += header.id.Length;

            buffer[_counter] = header.type;
            _counter++;

            Buffer.BlockCopy(header.len, 0, buffer, _counter, header.len.Length);
            _counter += header.len.Length;

            Buffer.BlockCopy(header.alen, 0, buffer, _counter, header.alen.Length);
            _counter += header.alen.Length;

            buffer[_counter] = header.packet_number;
            _counter++;

            Buffer.BlockCopy(header.version, 0, buffer, _counter, header.version.Length);
            _counter += header.version.Length;


            if (data_size > 0) // если данных нет, то появляется ошибка, так как одий байт данных всегда есть(так было сделать в функции get_len_packet чтобы избежать других ошибок) возможно это было лишьним
            {
                Buffer.BlockCopy(data, 0, buffer, _counter, data.Length);
                _counter += (int)data_size;
            }

            Buffer.BlockCopy(crc, 0, buffer, _counter, crc.Length);
            _counter += crc.Length;

            return _counter;
        }
        //============================================================================================================
        public void Receive_reset()
        {
            counter = 0;
            flag = 0;
        }
        //============================================================================================================
        public PACKET_STATUS Receive_byte(byte _data)
        {

            switch (flag)
            {
                case (int)Status.id:
                    if (_data != Constants.id[counter])// проверяем префикс
                    {
                        Receive_reset();
                    }
                    header.id[counter] = _data; counter++;
                    if (counter == header.id.Length) { flag = (int)Status.type; counter = 0; }
                    break;
                case (int)Status.type:
                    header.type = _data; counter++;
                    if (counter == 1) { flag = (int)Status.len; counter = 0; }
                    break;
                case (int)Status.len:
                    header.len[counter] = _data; counter++;
                    if (counter == header.len.Length)
                    {
                        flag = (int)Status.alen;
                        counter = 0;
                        data = default;
                        data_size = Get_len_pack() - (Constants.CRC_BYTE_SIZE + (uint)header_size);

                        data = new byte[data_size]; // создаем новый массив данных под нужный размер
                    }
                    break;
                case (int)Status.alen:
                    header.alen[counter] = _data; counter++;
                    if (counter == header.alen.Length) { flag = (int)Status.number; counter = 0; }
                    break;
                case (int)Status.number:
                    header.packet_number = _data; counter++;
                    if (counter == 1) { flag = (int)Status.version; counter = 0; }
                    break;
                case (int)Status.version:
                    header.version[counter] = _data; counter++;
                    if (counter == header.version.Length) { flag = (int)Status.data; counter = 0; }
                    break;
                case (int)Status.data:
                    if (data.Length > 0)
                    {
                        data[counter] = _data; counter++;
                        if (counter == data.Length) { flag = (int)Status.crc; counter = 0; }
                    }
                    else
                    {
                        flag = (int)Status.crc; counter = 0;
                    }
                    break;
                case (int)Status.crc:
                    crc[counter] = _data; counter++;
                    if (counter == crc.Length)
                    {
                        Receive_reset();
                        //flag = (int)Status.id; counter = 0;

                        if (_control_crc() != 0)
                        {
                            return PACKET_STATUS.MULTI_PACK;
                        }
                    }
                    break;
            }
            return PACKET_STATUS.NO_PACKETS;

        }

        //================================================================================================

    }
}