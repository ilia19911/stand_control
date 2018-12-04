using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace com_port
{
    static class Program
    {
        static byte i;
        static System.Threading.Thread twoTread;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        
        static void Main()
        {
            Form1.Com_init();
            System.Threading.Thread twoTread  =  new System.Threading.Thread(loop);
            twoTread.IsBackground = true;
            twoTread.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        static void loop()
        {
            while (1 >0)
            {

                  throttle_send();

            }
        }
        static void  throttle_send()
        {
            if (Form1.serialPort1.IsOpen == true)
            {
                try
                {
                    byte hiThrotByte = Convert.ToByte(Form1.throttle / 256);
                    byte LoThrotByte = Convert.ToByte(Form1.throttle % 256);
                    byte[] b = new byte[1];
                    b[0] = 0x55;
                    Form1.serialPort1.Write(b, 0, 1);
                    b[0] = 0xaa;
                    Form1.serialPort1.Write(b, 0, 1);
                    b[0] = 0x55;
                    Form1.serialPort1.Write(b, 0, 1);
                    b[0] = 0xaa;

                    Form1.serialPort1.Write(b, 0, 1);
                    b[0] = hiThrotByte;
                    Form1.serialPort1.Write(b, 0, 1);
                    b[0] = LoThrotByte;
                    Form1.serialPort1.Write(b, 0, 1);
                    b[0] = 0xcc;
                    Form1.serialPort1.Write(b, 0, 1);
                    b[0] = 0x66;
                    Form1.serialPort1.Write(b, 0, 1);
                }
                catch (InvalidOperationException)
                {

                }
            }
        }
    }
}



