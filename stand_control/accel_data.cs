using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace com_port
{


    public class Accel_data
    {
        //enum Type
        //{
        //    kalman,
        //    butterword
        //}
        /*
         * h- шум окружающей среды
         * f- шум измерений
         * q - влияние прерыдущего измерения на новое
         * r - количество измерений необходимых для установки значения
         */
        public FFT.KalmanFilterSimple1D filter_cos = new FFT.KalmanFilterSimple1D(f: 1, h: 1, q: 0.1, r: 10);
        public FFT.KalmanFilterSimple1D filter_sin = new FFT.KalmanFilterSimple1D(f: 1, h: 1, q: 0.1, r: 10);
        public FFT.KalmanFilterSimple1D filter_angle = new FFT.KalmanFilterSimple1D(f: 1, h: 1, q: 0.1, r: 10);
        public Accel_data()
        {
            filter_cos.SetState(0, 0); // Задаем начальные значение State и Covariance
            filter_sin.SetState(0, 0); // Задаем начальные значение State и Covariance

          //  clockwise_direction = false; // направление против часовой стрелки
        }

        public int all_angles_number; // хранит полное количество данных акселерометра

        public byte[]   accel_raw             = new byte[4000];
        public float[]  all_accel_data         = new float[10000];
        public double[] kalman_data         = new double[4000];
        public double[] butterwird_data     = new double[4000];
        public int[]    null_index             = new int[50];
        static public int      zero_count;
        public double   accel_measure_time; // хранит время потраченное на измерение данных акселерометром. из этого значения вычисляется скорость

        static double           phase; // переменная для записи результата работы программы. угол отклонения дисбаланса
        public static double    offset_angle;
        public static double    extremum_value;
        static public bool     clockwise_direction; //true если направление мотора по часовой стрелке 
     //   double          phase_cos;
     //   double          phase_sin;

        static int Byte_to_int(byte byteL, byte byteH)
        {
            int result = 0;
            result = byteL | (byteH << 8);
            return result;

        }

        static uint Int_to_byte(uint data, ref byte byteL, ref byte byteH)
        {
            byteL = (byte)(data & 0xFF);
            byteH = (byte)((data & 0xFF00) >> 8);
            return 2;
        }
        //================================================================================================
        static int Get_zero_mark(ref byte byteH)
        {
            int result = 0;
            if ((byteH & 64) > 0)
            {
                result = 1;
                byteH &= 191;
            }
            return result;
        }
        //================================================================================================
        static int Accel_Byte_to_int(byte byteL, byte byteH)
        {
            int result = 0;

            if ((byteH & 128) > 0)
            {
                byteH &= 127;

                result = byteL | (byteH << 8);
                result = 0 - result;
                return result;
            }
            else
            {
                result = byteL | (byteH << 8);
            }
            return result;
        }
        //==============================================================================================================================================================================
        public void Get_accel_data(byte[] data)
        {
            int result = 0;
            zero_count = 0; // нужно удалить, так как есть turn number
                            //rec_pack_struct.all_turn_number = 0;
            
            int angles = Byte_to_int(data[0], data[1]);
            accel_measure_time = Byte_to_int(data[2], data[3]);

                for (int i = 0; i < angles; i++)
                {
                    if (Get_zero_mark(ref data[5 + (i * 2)]) > 0)
                    {
                        null_index[zero_count] = i;

                        zero_count++;
                    }
                    result = Accel_Byte_to_int(data[4 + (i * 2)], data[5 + (i * 2)]);
                all_accel_data[i] = ((float)result/(256) )*(16); // переводим в g . 1.25 -костыль. нужно решить позже. данные отправляются как 12 битные, но по какой то причине они отличаются от истины
                }
                all_angles_number = angles;
                if (angles > 1)
                {
                    //Action action = () => Program.myForm.set_graphic(this);
                    //Program.myForm.Invoke(action);
                    //set_average_accel_data();
                }
            double speed = Calc_speed();
            return;
            }
        //==============================================================================================================================================================================
        public void Fill_butterword_data()
        {
           // double speed = Calc_speed();
            //5477
            FFT.FilterButterworth FBW = new FFT.FilterButterworth(((float)Protocol.measures.turns / 60), 5490, FFT.FilterButterworth.PassType.Lowpass,(float) 1/3); // создаем фильтр батерворда
            for (int q = 0; q < 2; q++)
            {
                for (int i = 0; i < all_angles_number; i++)
                {
                    FBW.Update(all_accel_data[i]);
                    butterwird_data[i] = FBW.Value;
                }
            }
        }
        //==============================================================================================================================================================================
        public void Fill_kalman_data()
        {
            FFT.KalmanFilterSimple1D kalman = new FFT.KalmanFilterSimple1D(f: 1, h: 1, q: 0.01, r: 20); // задаем F, H, Q и R // создаем фильтр калмана
            kalman.SetState(0, 0.01); // Задаем начальные значение State и Covariance // ставлю 0, потому что с другим значением график начитается с какой то хуйни

            for (var d = 0; d < all_angles_number; d++)
            {
                kalman.Correct(all_accel_data[d]); // Применяем алгоритм
                kalman_data[d] = kalman.State;
            }
          //  Fill_pure_sine();
        }
        //==============================================================================================================================================================================

        /*передается буфер с отфильтрованными значениями, и значение смещения фазы примененного фильтра 
         * 
         */
        private double Phase_definition(double[] buffer,float phase_displacement)
        {
            double angle = 0;

            angle = def_begin_grapfic(butterwird_data);
            if (angle < 0) return angle; ;
            angle += 360;

            if (clockwise_direction) // если движение по часовой стрелке . то считаем что по движению мотора вначале начинается отрицательное полушарие двигателя по значению акселерометра
            {
                angle = (angle - 180 + phase_displacement) % 360;
            }
            else
            {
                 angle = (angle - phase_displacement) % 360;
            }
            return angle;
        }
        //================================================================================================
        public double Calc_disbalance_angle() // пользовательская функция 
        {
            double result = 0;
            phase = Phase_definition(butterwird_data, 90);
            if (phase < 0) return phase;

            result = Calc_angle(phase);
            offset_angle = result;
            return result;
        }
        //================================================================================================
        private double Calc_angle(double angle)
        {
            double rad = 0;
            double result = 0;

            double cos = Math.Cos(ang_to_rad(angle));
            double sin = Math.Sin(ang_to_rad(angle));
            filter_cos.Correct(cos);
            filter_sin.Correct(sin);

            rad = Math.Asin(filter_sin.State);
            result = rad_to_ang(rad);

            if (filter_sin.State >= 0) // если угол до 180 градусов 
            {

                if (filter_cos.State < 0) //если угол в пределах 90 - 180
                {
                    result = 180 - result;
                }
            }
            else //если угол больше 180 гр.
            {
                if (filter_cos.State < 0)//если угол в пределах 180-270
                {
                    result = 180 - result;
                }
                else //если угол в пределах 270 - 360
                {
                    result = 360 + result;
                }
            }
            return result;
        }
        //================================================================================================
        private double def_begin_grapfic(double[] buffer) // функция определения основного угла смещения дисбаланса относительно нулевой отметки
        {
            double this_null    = def_rise_fault_graph(buffer, true);
            double angle_offset = 0;
            double angle_range = Calc_range_angle();

            if (this_null >= 0 )
            {
                angle_offset = ((angle_range * (this_null - null_index[0]) ) % 360); //получаем смещение в градусах относительно начала графика от нуля функции
                return angle_offset;
            }
            return -1;
        }
        //================================================================================================
        private int def_rise_fault_graph(double[] buffer, bool rise) // определение смены знака данных в масиве
        {
            double last_value   = 0;
            for(int i = 0; i < all_angles_number; i++)
            {
                if(buffer[i] == 0 ||  (last_value < 0 && buffer[i] > 0)  || (last_value > 0 && buffer[i] < 0))
                {
                    if (rise)
                    {
                        if (last_value < 0)
                        {
                            return i;
                        }
                    }
                    else
                    {
                        if (last_value > 0)
                        {
                            return i;
                        }
                    }
                }
                last_value = buffer[i];
            }
            return -1;
        }
        //================================================================================================
        //функция для определения экстремумы данных акселерометра. Используется как значение уровня вибрации на чистоте оборотов
        public void def_extremum()
        {
          //  extremum_value =  butterwird_data.Max();
            double result = 0;
                for(int i =0; i< all_angles_number; i++ )
                 {
                    if (result <  butterwird_data[i]) result = butterwird_data[i];
                 }
            extremum_value = result;
        }
        //================================================================================================
        private  float Calc_range_angle()
        {
            if (zero_count == 0) return 0;
            double  result = (float)360 /  ((float)null_index[zero_count - 1] / (float)(zero_count - 1) )  ;
            return (float)result; // получаем удельный эквивалент в градусах на одно измерение
        }
        //================================================================================================
        private  double Calc_speed()
        {
            if (zero_count == 0)              return 0;
            double result = 0;
            double turn_len = ((float)null_index[zero_count-1] - (float)null_index[0]) / (zero_count - 1);
            double meas_speed = (float)accel_measure_time / (float)all_angles_number;
            result = 60000 / (turn_len * meas_speed);

            Protocol.measures.turns = (int)result;
            return result;
        }
        //================================================================================================
        private double rad_to_ang(double rad)
        {
            return rad * 180 / Math.PI;
        }
        //================================================================================================
        private double ang_to_rad(double angle)
        {
            return angle * Math.PI / 180;
        }
        //================================================================================================

    }
}
