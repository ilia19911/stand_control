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
    public partial class window_settings : Form
    {
        public window_settings()
        {
            InitializeComponent();
            //textBox1.Focus();
            textBox1.Select();

        }

        private void window_settings_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public void button1_Click(object sender, EventArgs e)
        {
            save_click(sender, e);
            Dispose();
        }

        public event EventHandler save_click;

        public double get_throttle()
        {
            return Convert.ToDouble(textBox1.Text);
        }
        public void set_throttle(string input)
        {
            textBox1.Text = input;
        }

        public double get_speed()
        {
            return Convert.ToDouble(textBox2.Text);
        }

        public void set_speed(string input)
        {
            textBox2.Text = input;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
