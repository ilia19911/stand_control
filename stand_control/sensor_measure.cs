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

namespace com_port
{
    public partial class sensor_measure : Form
    {
        List <meas_string> new_meas;
        List<meas_string> reference;
        struct meas_string
        {
            double throttle;
            double Thrust;
            double Amp;
            double Volt;
            double gr;
            double vibration;
        };
        System.Timers.Timer aTimer = new System.Timers.Timer(2000);
        int throttle_multiplier = new int();
        int throttle_offset = 100;
        public sensor_measure()
        {
            InitializeComponent();
            ModifyMyListView(listView1);
            ModifyMyListView(listView2);
        }
        private void ModifyMyListView(ListView listView)
        {
            // Create a new ListView control.
            //ListView listView1 = new ListView();
            //listView1.Bounds = new Rectangle(new Point(10, 10), new Size(300, 200));

            // Set the view to show details.
            listView.View = View.Details;
            // Allow the user to edit item text.
            listView.LabelEdit = false;
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
            listView.Columns.Add("throttle(pwm)", -2, HorizontalAlignment.Left);
            listView.Columns.Add("Thrust(gr)", -2, HorizontalAlignment.Left);
            listView.Columns.Add("Amp(A)", -2, HorizontalAlignment.Left);
            listView.Columns.Add("Volt(V)", -2, HorizontalAlignment.Left);
            listView.Columns.Add("gr/W", -2, HorizontalAlignment.Left);
            listView.Columns.Add("vibration(g)", -2, HorizontalAlignment.Left);

        }
        void AddSubItem(ListViewItem thisItem)
        {
            listView1.Items.Add(thisItem);
            listView1.Items[listView1.Items.IndexOf(thisItem)].UseItemStyleForSubItems = false;
        }
        void start_measure()
        {

            // System.Threading.Thread.Sleep(1000);
            throttle_multiplier = 0;

            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;       
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Program.my_sensor_measure.Invoke(new Action(() => { Program.my_sensor_measure.AddSubItem(fill_sensor_data()); }));
            Protocol.throttle = throttle_multiplier * throttle_offset;
            throttle_multiplier += 1;
            if (throttle_multiplier > 10)
            {
                Program.my_sensor_measure.Invoke(new Action(() => { Program.my_sensor_measure.AddSubItem(fill_sensor_data()); })); //test
                throttle_multiplier = 0;
                aTimer.Stop();
                aTimer.Dispose();
            }
        }
        ListViewItem fill_sensor_data()
        {
            //ListViewItem item1 = new ListViewItem(Convert.ToString(throttle_multiplier), 0);
            ListViewItem item1 = new ListViewItem(Protocol.throttle.ToString());
            item1.SubItems[0].BackColor = comparison(item1.SubItems[0].Text, listView1.Items[0].SubItems[0]);
            // Place a check mark next to the item.
            item1.Checked = true;
           // item1.SubItems.Add(Convert.ToString(Protocol.throttle));
            item1.SubItems.Add(Convert.ToString(Protocol.measures.thrust));
            item1.SubItems.Add(Convert.ToString(Protocol.measures.curent));
            item1.SubItems.Add(Convert.ToString(Protocol.measures.voltage));
            item1.SubItems.Add(Convert.ToString(Protocol.measures.thrust));
            item1.SubItems.Add(Convert.ToString(Accel_data.extremum_value));
            item1.SubItems[0].BackColor = Color.Red;
            return item1;
        }
        Color comparison(dynamic value1, dynamic value2)
        {
            if (value1 = !value2)
                return Color.Red;
            else return Color.Green;
        }
        void stop_test()
        {

        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void begin_button_Click(object sender, EventArgs e)
        {
            start_measure();
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = ".xlsx"; // чтобы при сохранении добавилось расширение(чтобы пользователь видел расширение)
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = "xlsx";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            Excel thisxcel = new Excel();
            string filename = saveFileDialog1.FileName;
            // сохраняем текст в файл

            thisxcel.save_measure(listView1, saveFileDialog1.FileName);
                MessageBox.Show("Файл сохранен");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Excel thisxcel = new Excel();
            string path = "C:\\1.xlsx";
            thisxcel.load_file(ref listView2, path);
        }
    }
}
