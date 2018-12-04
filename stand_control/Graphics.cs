using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com_port
{
    public partial class Graphics : Form
    {
        public Graphics()
        {
            InitializeComponent();

            Average_graphic_init();

            FFT_graphic_init();

        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }
        private void chart3_Mouse_hover(object sender, EventArgs e)
        {
            MessageBox.Show("Включsdafgasdgadsgasdgasdg");
        }
        void Average_graphic_init()
        {
            //chart3.Series = new System.Windows.Forms.DataVisualization.Charting.SeriesCollection[3];
            chart3.ChartAreas[0].AxisX.ScaleView.Zoom(0, 1000);
            chart3.ChartAreas[0].AxisY.ScaleView.Zoom(-16, 16);
            chart3.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart3.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart3.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

            chart3.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart3.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart3.ChartAreas[0].AxisY.ScaleView.Zoomable = true;



            System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation myLine3 = new System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation();
            myLine3.Name = "myLine3";
            myLine3.X = 20;
            myLine3.Y = 20;
            chart3.Annotations.Add(myLine3);



            for (int i = 0; i < 36; i++)
            {
                chart3.Series[0].Points.AddXY(i, 0);
                chart3.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }

        }
        void FFT_graphic_init()
        {
            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(0, 1000);
            chart2.ChartAreas[0].AxisY.ScaleView.Zoom(0, 1400);
            chart2.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

            chart2.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].AxisY.ScaleView.Zoomable = true;



            System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation myLine2 = new System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation();
            myLine2.Name = "myLine2";
            myLine2.X = 20;
            myLine2.Y = 20;
            chart2.Annotations.Add(myLine2);



            for (int i = 0; i < 36; i++)
            {
                chart2.Series[0].Points.AddXY(i, 0);
                chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }
        }
         public void Set_graphics(Accel_data myAccel)
        {
            {
                Set_average_graphic(myAccel, 3, (dynamic)myAccel.all_accel_data, myAccel.all_angles_number);

                if (check_chekbox("Kalman"))
                {
                    Set_average_graphic(myAccel, 1, (dynamic)myAccel.kalman_data, myAccel.all_angles_number);
                }

                if (check_chekbox("Butterworth"))
                {
                    Set_average_graphic(myAccel, 2, (dynamic)myAccel.butterwird_data, myAccel.all_angles_number);
                }
            }
            
        }
        public void FFT_set_graphic(Accel_data myAccel)
        {
            // FFT.FFT myFFT = new FFT.FFT();
            chart2.Series[0].Points.Clear();
            System.Numerics.Complex[] c1;
            System.Numerics.Complex[] result;
            double[] resultInt;
            int number;
            if (myAccel.all_angles_number % 2 > 0) number = (myAccel.all_angles_number) - 1;
            else number = myAccel.all_angles_number;
            number = 512;

            c1 = new System.Numerics.Complex[number];
            resultInt = new double[number];

            for (int i = 0; i < number; i++)
            {
                c1[i] = myAccel.all_accel_data[i];
            }
            result = FFT.FFT.Fft(c1);

            for (int i = 0; i < number; i++)
            {
                // resultInt[i] = result[i].Real;
                resultInt[i] = result[i].Magnitude;
            }

            for (int i = 0; i < number / 2; i++)
            {
                chart2.Series[0].Points.AddXY(i * (5300 / number), resultInt[i]);
                chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            }


        }

        public void Set_average_graphic(Accel_data myAccel, int number, dynamic buffer, int graph_size)
        {
            int counter = 0;
            chart3.Series[0].Points.Clear();
            chart3.Series[number].Points.Clear();

            for (int i = 0; i < graph_size; i++)
            {
                if (i == myAccel.null_index[counter])
                {

                    if (counter < Accel_data.zero_count)
                    {
                        chart3.Series[0].Points.AddXY(myAccel.null_index[counter], -1);
                        chart3.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    }
                    counter++;

                }
                chart3.Series[0].Points.AddXY(i, 3);
                chart3.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

                chart3.Series[number].Points.AddXY(i, buffer[i]);
                chart3.Series[number].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }

            FFT_set_graphic(myAccel);
        }
        public bool check_chekbox(string name)
        {
            bool result = new bool();

            switch (name)
            {
                case ("Butterworth"):
                    result = checkBox3.Checked;
                    break;
                case ("Kalman"):
                    result = checkBox4.Checked;
                    break;
            }


            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Включите выведение графиков отфильтрованных данных акселерометра для визуального контроля уровня вибрации. Отфильтрованные данные отличаются от неотфильтрованных наглядностью. Не отфильтрованные данные имеют большие шумы, и не дают возможности для анализа. Вы имеете возможность включть два вида отфильтрованных данных; с  помощью фильтра Калмана и фильтра Батерворда. Данные, полученные после фильтрации Калмана не имеют смещения фазы, но имеют заметный шум. Фильтр Батерворда , напротив, позволяет получть чистый синус, но имеет смещение фазы 90 градусов");
        }
    }
}
