using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

using System.Collections.Generic;
using file_manager;

namespace com_port
{


    public partial class Form1 : Form
    {
        //Bitmap MyImage = new Bitmap("D://PROJECTS//copterStand//программа тестирования c#//com_port//com_port/1.bmp");

        
      //  TransparentControl hh = new TransparentControl();

       // PictureBox pictureBox2 = new PictureBox();
       // PictureBox pictureBox3 = new PictureBox();

        System.Drawing.Graphics arrow; // графика для отрисовки фона
        System.Drawing.Graphics grid; // графика для отрисовки динамики

        double min_disbalance = 1;
        double mid_disbalance = 2;
        double max_disbalance = 3;


        //================================================================================================
        public void Set_test_text(float angle)
        {
            test_text.Text = Convert.ToString(angle);
        }
        //================================================================================================
        public void Set_version_text(string str)
        {
            version.Text = str;
        }
        public void Set_test_text(string str)
        {
            test_text.Text = str;
        }
        //================================================================================================
        public void Set_average_text()
        {

            curent.Text = Convert.ToString(Protocol.measures.curent); 
            voltage.Text = Convert.ToString(Protocol.measures.voltage);
            thrust.Text = Convert.ToString(Protocol.measures.thrust); 
            turns.Text = Convert.ToString(Protocol.measures.turns);

        }
        //================================================================================================
        public void Init_image()
        {
            grid  = System.Drawing.Graphics.FromHwnd(pictureBox1.Handle);
            arrow = System.Drawing.Graphics.FromHwnd(pictureBox1.Handle);

            Set_image();
        }
        //================================================================================================
        public void Set_font(System.Drawing.Graphics myGraphic)
        {
            Pen BlackPen = new Pen(Color.Purple, 1);
            int cx = 175;
            int cy = 175;
            int hlen = 155;
            //отрисовка кругов
            myGraphic.DrawEllipse(BlackPen, 20, 20, 310, 310);
            myGraphic.DrawEllipse(BlackPen, 70, 70, 210, 210);
            myGraphic.DrawEllipse(BlackPen, 120, 120, 110, 110);

            // отрисовка линий градусов
            for (int i = 0; i < 24; i++)
            {
                double _angle = _angle = (15 * i);
               
                int x = (int)(hlen * Math.Sin(Math.PI * _angle / 180));
                int y = (int)(hlen * Math.Cos(Math.PI * _angle / 180));

                x = cx + x;
                y = cy - y;

                myGraphic.DrawLine(BlackPen, x, y, cx, cy);

                Set_clok(myGraphic , _angle, cx-10, cy-5, hlen+10);
            }
        }
        //================================================================================================
        public void Set_clok(System.Drawing.Graphics myGraphic, double angle, int cx, int cy, int hlen)
        {
            double text = angle;
            if (Accel_data.clockwise_direction)
                text = 360 - angle;
            myGraphic.DrawString(Convert.ToString(text), new Font("Arial", 8), Brushes.Black, new PointF(cx + (int)(hlen  * Math.Sin(Math.PI * angle / 180)), cy + (int)(hlen * Math.Cos(Math.PI * angle / 180))));
        }
        //================================================================================================
        public void Set_image()
        {
            Pen myPen = new Pen(Color.Green, 3); ;

            if(Accel_data.extremum_value< min_disbalance)
                myPen = new Pen(Color.Green, 3);
            if (Accel_data.extremum_value < mid_disbalance && Accel_data.extremum_value >= min_disbalance)
                myPen = new Pen(Color.Yellow, 3);
            if (Accel_data.extremum_value < max_disbalance && Accel_data.extremum_value >= mid_disbalance)
                myPen = new Pen(Color.Red, 3);
            arrow.Clear(Color.White);
            Set_font(arrow);

            double angle;
            if (Accel_data.clockwise_direction)
                angle = (180 + Accel_data.offset_angle) % 360;
            else
                angle = 360 - (180 + Accel_data.offset_angle) % 360;

            int[] coord = new int[2];
            int cx = 175;
            int cy = 175;
            int hlen = (int)(155*(Accel_data.extremum_value/ mid_disbalance));
            if (angle >= 0 && angle <= 180)
            {
                coord[0] = cx + (int)(hlen * Math.Sin(Math.PI * angle / 180));
                coord[1] = cy - (int)(hlen * Math.Cos(Math.PI * angle / 180));
            }
            else
            {
                coord[0] = cx - (int)(hlen * -Math.Sin(Math.PI * angle / 180));
                coord[1] = cy - (int)(hlen * Math.Cos(Math.PI * angle / 180));
            }

            //отрисовка указателя
            arrow.DrawLine(myPen, coord[0], coord[1], cx, cy);
          //  DrawImage
        }
        //================================================================================================
        public Form1()
        {
            InitializeComponent();
            GetAvailablePorts();
            Init_image();
            checkBox1.Checked = true;
            textBox2.Text = "1";
        }
        //================================================================================================
        void GetAvailablePorts()
        {
            comboBox1.Items.AddRange(serial.Get_name_ports());
        }
        //================================================================================================
        private void Button_connect_Click(object sender, EventArgs e)
        {
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    MessageBox.Show("порт или скорость порта не выбраны");
                }
                else
                {
                    if (serial.Init(comboBox1.Text, Convert.ToInt32(comboBox2.Text)))
                    {
                        comboBox1.Enabled = false;
                        comboBox2.Enabled = false;
                        button_connect.Enabled = false;

                       

                         Program.twoTread = new System.Threading.Thread(Program.myProtocol.loop);
                         Program.twoTread.IsBackground = true;
                         Program.twoTread.Start();

                        Init_image();
                    }
                    else
                    {
                         MessageBox.Show("не возможно подключиться к выбранному порту");
                    }
                }
        }
        //================================================================================================
        private void Button_close_Click(object sender, EventArgs e)
        {
            serial.close_port();
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            button_connect.Enabled = true;
            Program.twoTread.Abort();
        }
        //================================================================================================
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Protocol.throttle = trackBar1.Value;

            label2.Text = Convert.ToString(trackBar1.Value );
        }
        //================================================================================================
        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        public void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            Protocol.adc_value = (byte)trackBar2.Value;
            Protocol.Set_tx_flag(Packet_type.adc_set_request, true);

            label9.Text = Convert.ToString(trackBar2.Value);
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            Set_clockwise(true);
            if (checkBox2.Checked == false)
                checkBox1.Checked = true;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                Set_clockwise(false);
            if (checkBox1.Checked == false)
                checkBox2.Checked = true;
        }
        public void Set_clockwise(bool clocwise)
        {
            if(clocwise)
            {
                checkBox2.CheckState = CheckState.Unchecked;
                Accel_data.clockwise_direction = true;
            }
            else
            {
                checkBox1.CheckState = CheckState.Unchecked;

                Accel_data.clockwise_direction = false;
            }
        }
        private void test_text_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.graphics = new Graphics();
            Program.graphics.Hide();
            Program.graphics.Show();
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
        public void Set_max_accel(dynamic text)
        {
            
            textBox1.Text = Convert.ToString(text) ;
            double value = 100 * (Convert.ToDouble(text) / 4);
            if (value > 100) value = 100;
            progressBar1.Value = (int)value;
            
            if (Convert.ToDouble(text) < min_disbalance) ModifyProgressBarColor.SetState(progressBar1, 1);
            if (Convert.ToDouble(text) >= min_disbalance && Convert.ToDouble(text) < mid_disbalance) ModifyProgressBarColor.SetState(progressBar1, 3);
            if (Convert.ToDouble(text) >= max_disbalance) ModifyProgressBarColor.SetState(progressBar1, 2);  
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox2.Text) < 1) textBox2.Text = "1";
            if (Convert.ToInt32(textBox2.Text) > 10) textBox2.Text = "10";
            Protocol.request_measure_turns = Convert.ToByte(textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.my_sensor_measure = new sensor_measure();
            Program.my_sensor_measure.Hide();
            Program.my_sensor_measure.Show();

        }
    }
    //================================================================================
    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }

    //================================================================================

    public class TransparentControl : Control
    {
        private readonly Timer refresher;
        private Image _image;

        public TransparentControl()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            refresher = new Timer();
            refresher.Tick += TimerOnTick;
            refresher.Interval = 50;
            refresher.Enabled = true;
            refresher.Start();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        protected override void OnMove(EventArgs e)
        {
            RecreateHandle();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (_image != null)
            {
                e.Graphics.DrawImage(_image, (Width / 2) - (_image.Width / 2), (Height / 2) - (_image.Height / 2));
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //Do not paint background
        }

        //Hack
        public void Redraw()
        {
            RecreateHandle();
        }

        private void TimerOnTick(object source, EventArgs e)
        {
            RecreateHandle();
            refresher.Stop();
        }

        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                RecreateHandle();
            }
        }
    }


}
