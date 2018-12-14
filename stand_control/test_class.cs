using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
namespace com_port
{
    public struct meas_string
    {
        public double throttle;
        public double turns;
        public double Thrust;
        public double Amp;
        public double Volt;
        public double capacity;
        public double gr_W;
        public double vibration;
        public double speed;

        public meas_string(double throt, double tur, double thru, double A, double V, double gr,double cap, double vib, double sp)
        {
            throttle = throt; turns = tur; Thrust = thru; Amp = A; Volt = V; gr_W = gr; vibration = vib; speed = sp; capacity = cap;
        }
    };

    class test_class

    {
        public bool blade_opening;
        public bool cyrcle_mode;

        public event EventHandler fill_items;

        System.Timers.Timer aTimer          = new System.Timers.Timer(2000);
        public test_struct test_status      = new test_struct();
        public List<meas_string> new_meas   = new List<meas_string>();
        public List<meas_string> reference  = new List<meas_string>();


 

        public struct test_struct
        {
            public int Old_value;
            public int New_value;
            public int state;
            public int step_number;
            public int last_step;
            public double fade;
            public double speed;
            public enum status_enum
            {
                Off,
                Slow,
                Fast,
                Break,
                Blade_opening
            }
        };
        public test_class()
        {
           
        }
        //======================================================================
        public void add_measure(ref List<meas_string> this_list)
        {
            if (cyrcle_mode == true) return;

            

            meas_string this_measure    = new meas_string();
            this_measure.throttle       = Protocol.throttle;
            this_measure.turns          = Protocol.measures.turns;
            this_measure.Thrust         = Protocol.measures.thrust;
            this_measure.Amp            = Protocol.measures.curent;
            this_measure.Volt           = Protocol.measures.voltage ;
            this_measure.gr_W           = Protocol.measures.g_W;
            this_measure.capacity       = Protocol.measures.capacity;
            this_measure.vibration      = Accel_data.extremum_value;
            this_measure.speed          = test_status.speed;
            this_list.Add(this_measure);
        }

        //======================================================================
        public void start_measure()
        {

            // System.Threading.Thread.Sleep(1000);

            if (reference.Count == 0)
            {
                MessageBox.Show("Программа испытаний пуста, добавьте шаги, или загрузите программу");
                return;
            }
            new_meas = null;
            new_meas = new List<meas_string>();

            test_status.step_number = 0;
            state_defination();
            if (blade_opening == true)
            {
                test_status.state = (int)test_struct.status_enum.Blade_opening;
            }
            else
            {
                test_status.state = (int)test_struct.status_enum.Slow;
            }
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled   = true;
        }
        //======================================================================
        public void stop_test()
        {
            test_status.state = (int)test_struct.status_enum.Off;
        }
        //======================================================================
        public void set_next_step(Object source, ElapsedEventArgs e)
        {
            test_status.fade = 0;
            Protocol.throttle = test_status.New_value;
            test_status.step_number++;
            if (test_status.step_number < reference.Count)
            {
                state_defination();
                Program.my_sensor_measure.Invoke(new Action(() => { add_measure(ref new_meas); })); //test
                Program.my_sensor_measure.Invoke(new Action(() => { fill_items(source, e); })); //test

            }
            else
            {
                if (cyrcle_mode == true)
                {
                    test_status.step_number = 0;
                    state_defination();
                }
                else
                {
                    // aTimer.Interval = 1;
                    Program.my_sensor_measure.Invoke(new Action(() => { add_measure(ref new_meas); })); //test
                    Program.my_sensor_measure.Invoke(new Action(() => { fill_items(source, e); })); //test
                    test_status.state = (int)test_struct.status_enum.Off;
                }
            }

            Pause_Handler();
        }
        //======================================================================
        public void smooth_change(Object source, ElapsedEventArgs e)
        {
            Protocol.Construct_send_packet(Packet_type.throttle_pack);
            if (test_status.speed == 0) // если время перехода 0;
            {
                set_next_step(source, e);
            }

            else
            {
                aTimer.Interval = test_status.speed / (1000 / (0.04 * 1000));
                Protocol.throttle = (int)value_calc();
                test_status.fade += 0.04;
                if (test_status.fade > 1)
                {
                    set_next_step(source, e);
                }
            }
        }
        //======================================================================
        public void state_defination()
        {
            test_status.speed           = reference[test_status.step_number].speed;
            test_status.Old_value       = test_status.New_value;
            test_status.New_value       = (int)reference[test_status.step_number].throttle;
        }
        //======================================================================
        public double value_calc()
        {
            return test_status.Old_value + ((test_status.New_value - test_status.Old_value) * test_status.fade);
        }
        //======================================================================
        //======================================================================
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            switch (test_status.state)
            {
                case (int)test_struct.status_enum.Off:
                    {
                        aTimer.Interval = 10;

                         if (Protocol.throttle > 5) Protocol.throttle -= 5;
                         else
                         {
                             Protocol.throttle = 0;
                             aTimer.Stop();
                             aTimer.Close();
                             aTimer.Enabled = false;
                             aTimer.Dispose();
                         }
                        break;
                    }
                case (int)test_struct.status_enum.Slow:
                    {
                        smooth_change(source, e);
                        break;
                    }
                case (int)test_struct.status_enum.Blade_opening:
                    {
                        Protocol.throttle       = 1000;
                        aTimer.Interval         = 1000;
                        test_status.speed       = 700;
                        test_status.Old_value   = Protocol.throttle;
                        test_status.state       = (int)test_struct.status_enum.Slow;
                        break;
                    }
            }
        }

        //======================================================================
        public void Pause_Handler()
        {
            aTimer.Interval = 500;

        }

        
        //======================================================================
        public void fill_leap_test(ref List<meas_string> this_list)
        {
            this_list.Clear();
            this_list.Add(new meas_string(200, 0, 0, 0, 0, 0, 0, 0,0));
            this_list.Add(new meas_string(300, 0, 0, 0, 0, 0, 0, 0,0));
        }
        //======================================================================
        public void fill_smooht_test(ref List<meas_string> this_list)
        {
            this_list.Clear();
            this_list.Add(new meas_string(200, 0, 0, 0, 0, 0, 0, 0,1000));
            this_list.Add(new meas_string(300, 0, 0, 0, 0, 0, 0, 0,1000));
            this_list.Add(new meas_string(400, 0, 0, 0, 0, 0, 0, 0,1000));
            this_list.Add(new meas_string(500, 0, 0, 0, 0, 0, 0, 0,1000));
            this_list.Add(new meas_string(600, 0, 0, 0, 0, 0, 0, 0,1000));
            this_list.Add(new meas_string(700, 0, 0, 0, 0, 0, 0, 0,1000));
            this_list.Add(new meas_string(800, 0, 0, 0, 0, 0, 0, 0,1000));
            this_list.Add(new meas_string(900, 0, 0, 0, 0, 0, 0, 0,1000));
            this_list.Add(new meas_string(1000, 0, 0, 0, 0, 0, 0, 0, 1000));
        }
        //======================================================================
    }
}
