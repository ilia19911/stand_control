using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Globalization;
using System.Timers;



namespace com_port
{
    class serial
    {
        static public System.IO.Ports.SerialPort Myserial;

        static public bool Init(String portName, int baudRate)
        {
            try
            {
                Myserial = new System.IO.Ports.SerialPort();
                Myserial.PortName = portName;
                Myserial.BaudRate = baudRate;
                Myserial.StopBits = StopBits.One;
                Myserial.DataBits = 8;
                Myserial.Parity = Parity.None;
                Myserial.Open();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            return true;
        }
        static public String[] Get_name_ports()
        {
            return SerialPort.GetPortNames();
        }
        static public void close_port()
        {
            Myserial.Close();
        }
        static void Send(byte[] buffer, int number)
        {
            Myserial.Write(buffer, 0, number);
        }
    }
}

