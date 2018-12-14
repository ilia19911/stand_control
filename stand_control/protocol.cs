
using System;
using System.Timers;
using System.Collections.Generic;

using System.Numerics;

namespace com_port
{
    public enum Packet_type
    {
        _null,
        throttle_pack,
        throttle_answer,

        accel_measure_request,
        accel_answer,
        accel_measure_repeated_request,

        request_averaged_measure,
        averaged_measure,

        adc_set_request,
        adc_answer
    };

    enum Receive_status
    {
        BUFFER_NOT_BUSY         = 0x00,
        BUFFER_RECEIVE_HEADER   = 0x01,
        BUFFER_RECEIVE_DATA     = 0x03,
        BUFFER_BUSY             = 0x04
    }

    public struct Average_measure
    {
        public double thrust ;
        public int turns  ;
        public double voltage;
        public double curent;
        public double capacity;
        public double g_W;
    }

    class Protocol
    {
        static FFT.KalmanFilterSimple1D Average_amp = new FFT.KalmanFilterSimple1D(f: 1, h: 1, q: 0.01, r: 10); // задаем F, H, Q и R // создаем фильтр калмана


        public Protocol()
        {
            Average_amp.SetState(0, 0.01); // Задаем начальные значение State и Covariance // ставлю 0, потому что с другим значением график начитается с какой то хуйни
            
            flag                    = 0;
            throttle                = 0;
            request_measure_turns   = 1;
            adc_value               = 70;
            tx_flag                 = 0;
            len_rcv                 = 0;
        }


       static public Accel_data myAccel         = new Accel_data();
       static Trans_protocol my_tx_Packets      = new Trans_protocol();
       static Trans_protocol my_rx_Packets      = new Trans_protocol();

       //временные переменные
       static public byte request_measure_turns;
       static public byte adc_value;
       static public int throttle;

       static int        flag;
       static int        tx_flag;
       static int        len_rcv;
       static public     Average_measure           measures = new Average_measure();
      // static public Multipac myMultipack = new Multipac();
        //================================================================================================
        static void Packet_send(Object source, ElapsedEventArgs e)
        {
            Program.myForm.Invoke(new Action(() => { Program.myForm.Set_test_text((float)Accel_data.offset_angle); }));
            

            Program.myForm.Invoke(new Action(() => { Program.myForm.Set_max_accel(Accel_data.extremum_value); }));

            Set_tx_flag(Packet_type.accel_measure_request, true);
            Set_tx_flag(Packet_type.request_averaged_measure, true);
           // Set_tx_flag(Packet_type.accel_measure_repeated_request, true);
            Construct_send_packet(Packet_type.throttle_pack);
            for (int i = 1; i < 10; i++)
            {
                if (Get_tx_flag((Packet_type)i) > 0)
                {
                    Construct_send_packet((Packet_type)i);
                    Set_tx_flag((Packet_type)i, false);
                }
            }
        }
        //================================================================================================
           public void loop()
        {
           System.Timers.Timer aTimer;
           aTimer = new System.Timers.Timer(100);
           aTimer.Elapsed += Packet_send;
           aTimer.AutoReset = true;
           aTimer.Enabled = true;
          
           while (1 > 0)
           {
               Receive_date();
                
                if (serial.Myserial.IsOpen == false)
               {
                   Program.twoTread.Abort();
               }
            }

        }
        //================================================================================================
         public static void Construct_send_packet(Packet_type type) //запрос измерений акселерометра
        {
            if (serial.Myserial.IsOpen == true)
            {
                my_tx_Packets.Construct_header( type);
     
                switch (type)
                {
                    case Packet_type.throttle_pack:
                        byte[] ThrotByte = new byte[2];
                        ThrotByte[0] = Convert.ToByte(throttle / 256);
                        ThrotByte[1] = Convert.ToByte(throttle % 256);
                        my_tx_Packets.Write_data_tx( ThrotByte, 2);
                        break;
                    case Packet_type.accel_measure_request:
                        my_tx_Packets.Write_data_tx(request_measure_turns);
                        break;
                    case Packet_type.accel_measure_repeated_request:
                        my_tx_Packets.Write_data_tx(3);
                        break;
                    case Packet_type.request_averaged_measure:
                        my_tx_Packets.Write_data_tx(0);
                        break;
                    case Packet_type.adc_set_request:
                        my_tx_Packets.Write_data_tx(adc_value);
                        break;
                    default: return;
                }
                my_tx_Packets.Send_protocol_packet();
            }
        }
        //================================================================================================
        static void Receive_date()
        {
            try
            {
                if (serial.Myserial.IsOpen == true)
                {
                    {
                        Int32 num = serial.Myserial.BytesToRead;
 
                        if (num > 0)
                        {
                            byte temp_c = Convert.ToByte(serial.Myserial.ReadByte());
                            if(_protocol_processByte(temp_c)>0)
                            {
                                //_pack_handler();
                            }
                        }
                    }
                }
            }
            catch(System.UnauthorizedAccessException)
            {
 
            }
        }
        //================================================================================================
        static int _protocol_processByte(byte new_byte)
        {
           if( my_rx_Packets.Receive_byte(new_byte) != Packet_struct.PACKET_STATUS.NO_PACKETS)
            {
                return my_rx_Packets.Multipack_receive();
            }
            return 0;
        }
        //================================================================================================
        static void _stop_receive()
        {
            flag = (int)Receive_status.BUFFER_NOT_BUSY;
            len_rcv = 0;
            serial.Myserial.DiscardInBuffer();
        }
        //================================================================================================
        static void set_version()
        {
            string version = String.Format("{0},{1}", Convert.ToString(my_rx_Packets.header.version[0]), Convert.ToString(my_rx_Packets.header.version[1]));
            Program.myForm.Invoke(new Action(() => { Program.myForm.Set_version_text(version); }));
        }
        //================================================================================================
        public static void _pack_handler(byte[] data)
        {

            set_version();
            switch (my_rx_Packets.header.type)
            {
                case (byte)Packet_type.throttle_answer: //просто ответ на пакет со значением дроссельной заслонки. можно сделать значек connect вверху окна
                                                        //while (1 == 1) { };
                    break;
 
                case (byte)Packet_type.accel_answer:
                    myAccel.Get_accel_data(data);
                    myAccel.Fill_kalman_data();
                    myAccel.Fill_butterword_data();
                    myAccel.Calc_disbalance_angle();
                    myAccel.def_extremum();

                    
                    Program.myForm.Invoke(new Action(() => { Program.myForm.Set_average_text(); }));

                    Program.myForm.Invoke(new Action(() => { Program.myForm.Set_image(); }));
                    try
                    {
                        Program.graphics.Invoke(new Action(() => { Program.graphics.Set_graphics(myAccel); }));
                    }
                    catch(System.InvalidOperationException){}
                    break;
 
                case (byte)Packet_type.averaged_measure:
                    Get_average_data();
                    break;
                case (byte)Packet_type.adc_answer:
                    {
                      //  int i;
                    }
                    break;
 
                default: break;
            }
        }
        //================================================================================================
         static void Get_average_data()
        {
            Average_amp.Correct(Byte_to_int(my_rx_Packets.data[7], my_rx_Packets.data[8])); // Применяем алгоритм

            measures.thrust  = (double)(my_rx_Packets.data[0] | (my_rx_Packets.data[1] << 8) | (my_rx_Packets.data[2] << 16));
            //measures.turns = Byte_to_int(my_rx_Packets.structure.data[3], my_rx_Packets.structure.data[4]);
            measures.voltage = (double)(Byte_to_int(my_rx_Packets.data[5], my_rx_Packets.data[6]))/1000;
            measures.voltage -= 0.190;
            measures.curent  = Convert.ToDouble(Average_amp.State)/1000;
            measures.capacity = measures.curent * measures.voltage;
            measures.g_W     = ( measures.thrust/ measures.capacity );
            try
            {
                Program.myForm.Invoke(new Action(() => { Program.myForm.Set_average_text(); }));
            }
            catch (System.ObjectDisposedException)
            {
 
            }
            catch (System.InvalidOperationException)
            {

            }
        }
        //================================================================================================
        static int Byte_to_int(byte byteL, byte byteH)
        {
            int result = 0;
            result = byteL | (byteH << 8);
            return result;
        }
        //================================================================================================
        static uint Int_to_byte(uint data, ref byte byteL, ref byte byteH)
        {
            byteL = (byte)(data & 0xFF);
            byteH = (byte)((data & 0xFF00) >>8);
            return 2;
        }

        //================================================================================================
       static public void Set_tx_flag(Packet_type type, bool enable)
       {
          if(enable) tx_flag |= (0x01 << (int)type);
          else tx_flag &= ~((int)type);
       }
        //================================================================================================
        static int Get_tx_flag(Packet_type type)
        {
            int result = ( (tx_flag>>(int)type) & 0x01 );
            return result;
 
        }
    }
}
