using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
//using System.Windows.SaveFileDialog;
using file_manager;
//using static com_port.window_settings;
using static  com_port.test_class;

namespace com_port
{
    public partial class window_test : Form
    {


        test_class      MyTest   = new test_class();
        window_settings f2       = new window_settings();// форма окна настроек

        //======================================================================
        public window_test()
        {
            InitializeComponent();
            ModifyMyListView(listView1, false);
            ModifyMyListView(listView2, true);

            listView2.MouseDoubleClick += ListView2_MouseDoubleClick;
            FormClosing += Sensor_measure_FormClosing;
            MyTest.fill_items += MyTest_fill_items;
        }

        private void MyTest_fill_items(object sender, EventArgs e)
        {
            fill_item(MyTest.new_meas, listView1);
           // throw new NotImplementedException();
        }

        private void Sensor_measure_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
            MyTest.stop_test();
        }

        private void F2_save_click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            int i = listView2.SelectedIndices[0];

            //ListViewItem item1;

            meas_string result = MyTest.reference[i];
            result.throttle = f2.get_throttle();
            result.speed = f2.get_speed();
            MyTest.reference[i] = result;
            fill_item(MyTest.reference, listView2);
        }

        private void ListView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();

            f2 = new window_settings();// форма окна настроек
            f2.save_click += F2_save_click;

            int i = listView2.SelectedIndices[0];
            meas_string result = MyTest.reference[i];

            f2.set_throttle(result.throttle.ToString());
            f2.set_speed (result.speed.ToString());

            f2.Hide();
            f2.Show();


        }

        //======================================================================
        private void ModifyMyListView(ListView listView, bool edit)
        {
            // Create a new ListView control.
            //ListView listView1 = new ListView();
            //listView1.Bounds = new Rectangle(new Point(10, 10), new Size(300, 200));

            // Set the view to show details.
            listView.View = View.Details;
            // Allow the user to edit item text.
            listView.LabelEdit = edit;
            
            // Allow the user to rearrange columns.
            listView.AllowColumnReorder = false;
            // Display check boxes.
            listView.CheckBoxes = false;
            // Select the item and subitems when selection is made.
            listView.FullRowSelect = true;
            // Display grid lines.
            listView.GridLines = true;
            // Sort the items in the list in ascending order.
            listView.Sorting = SortOrder.None;

            // Create columns for the items and subitems.
            // Width of -2 indicates auto-size.
            //listView.Columns.Add("№", -2, HorizontalAlignment.Left);
            listView.Columns.Add("throttle(pwm)",     -2, HorizontalAlignment.Left);
            listView.Columns.Add("turns(t/m)",        -2, HorizontalAlignment.Left);
            listView.Columns.Add("Thrust(gr)",        -2, HorizontalAlignment.Left);
            listView.Columns.Add("Amp(A)",            -2, HorizontalAlignment.Left);
            listView.Columns.Add("Volt(V)",           -2, HorizontalAlignment.Left);
            listView.Columns.Add("gr/W",              -2, HorizontalAlignment.Left);
            listView.Columns.Add("vibration(g)",      -2, HorizontalAlignment.Left);
            listView.Columns.Add("Speed(ms)",         -2, HorizontalAlignment.Left);

        }
        //======================================================================
        void AddSubItem(ListViewItem thisItem)
        {
            listView1.Items.Add(thisItem);
            listView1.Items[listView1.Items.IndexOf(thisItem)].UseItemStyleForSubItems = false;
        }



        //======================================================================


        //======================================================================
        private void fill_item(List<meas_string> this_list, ListView this_listView)
        {
               this_listView.Items.Clear(); //чистим ListView
               
              
               // Place a check mark next to the item.
            foreach (var meas in this_list)
            {

                ListViewItem item1 = new ListViewItem(Convert.ToString(meas.throttle));
                item1.Checked = true;

                item1.SubItems.Add(Convert.ToString(meas.turns));
                item1.SubItems.Add(Convert.ToString(meas.Thrust));
                item1.SubItems.Add(Convert.ToString(meas.Amp));
                item1.SubItems.Add(Convert.ToString(meas.Volt));
                item1.SubItems.Add(Convert.ToString(meas.gr_W));
                item1.SubItems.Add(Convert.ToString(meas.vibration));
                item1.SubItems.Add(Convert.ToString(meas.speed));

                item1.UseItemStyleForSubItems = false;

                item1.SubItems[1].BackColor = Color.Red; //comparison(item1.SubItems[0].Text, listView1.Items[0].SubItems[0]);

                this_listView.Items.Add(item1);
            }
        }
        //======================================================================
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void begin_button_Click(object sender, EventArgs e)
        {
            MyTest.start_measure();
        }
        //======================================================================
        private void save_button_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName         = ".xlsx"; // чтобы при сохранении добавилось расширение(чтобы пользователь видел расширение)
            saveFileDialog1.CheckPathExists  = true;
            saveFileDialog1.AddExtension     = true;
            saveFileDialog1.DefaultExt       = "xlsx";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            Excel thisxcel = new Excel();
            string filename = saveFileDialog1.FileName;
            // сохраняем текст в файл

            thisxcel.save_measure(MyTest.new_meas, saveFileDialog1.FileName);
                MessageBox.Show("Файл сохранен");
        }
        //======================================================================
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName         = ".xlsx"; // чтобы при сохранении добавилось расширение(чтобы пользователь видел расширение)
            openFileDialog1.CheckPathExists  = true;
            openFileDialog1.AddExtension     = true;
            openFileDialog1.DefaultExt       = "xlsx";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;


            Excel thisxcel = new Excel();
            string path = openFileDialog1.FileName;
            thisxcel.load_file(ref MyTest.reference, path);
            MyTest.reference.Clear();
            fill_item(MyTest.reference, listView2);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            MyTest.fill_leap_test(ref MyTest.reference);
            fill_item(MyTest.reference, listView2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MyTest.fill_smooht_test(ref MyTest.reference);
            fill_item(MyTest.reference, listView2);
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            meas_string this_measure = new meas_string();
            this_measure.throttle   = 0;
            this_measure.turns      = 0;
            this_measure.Thrust     = 0;
            this_measure.Amp        = 0;
            this_measure.Volt       = 0;
            //this_measure.gr_W =
            this_measure.vibration  = 0;
            this_measure.speed      = 1000;

            MyTest.reference.Add(this_measure);

            fill_item(MyTest.reference, listView2);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try // если элемент не выбран
            {
                int i = 0;
                foreach(int obj in listView2.SelectedIndices)
                {
                    MyTest.reference.RemoveAt(obj-i);
                    i++;
                }

               // MyTest.reference.RemoveAt(i);
                fill_item(MyTest.reference, listView2);
            }
            catch(System.ArgumentOutOfRangeException)
            {

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
                MyTest.blade_opening = true;
            else
                MyTest.blade_opening = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MyTest.stop_test();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                MyTest.cyrcle_mode = true;
            else
                MyTest.cyrcle_mode = false;
        }
    }
}
