using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using System.Windows.Forms;
//using System.IO.Ports;
//using System.ComponentModel;

namespace com_port
{
     class Program
    {
        static public System.Threading.Thread twoTread;
        //static public System.Threading.Thread tрreeTread;
        static public Form1 myForm = new Form1();
        static public Graphics graphics   = new Graphics();
        static public Protocol myProtocol = new Protocol();
        static public sensor_measure my_sensor_measure = new sensor_measure();
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        /// 
        [STAThread]

        static void Main()
        {


            Application.EnableVisualStyles();
           // Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(myForm);
        }
    }
}



