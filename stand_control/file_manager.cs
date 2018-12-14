using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Windows.Forms;
using static com_port.window_test;
using static com_port.test_class;


namespace file_manager
{
    public class headings
    {
        public struct Param
        {
           public  Param(string A, string B)
            {
                column = A;
                Name   = B;
            }
            public string column;
            public string Name;
        };
        public Param throttle    = new Param("A", "Дроссель(%)");
        public Param turns       = new Param("B", "Обороты(об./мин.)");
        public Param Thrust      = new Param("C", "Тяга(г)");
        public Param Amp         = new Param("D", "Ампер(А)");
        public Param Volt        = new Param("E", "Вольт(В)");
        public Param gr_W        = new Param("F", "г/Вт");
        //public Param capacity    = new Param("F", "г/Вт");
        public Param vibration   = new Param("G", "Вибрация(g)");
        public Param Speed       = new Param("H", "Speed(ms)");
        public Param meas_number = new Param("I", "количество измерений");
    }


    public class Excel
    {
        string excel_file_name = "результаты испытаний";
        headings my_headings = new headings();
 
        public Excel()
        {

        }
        void excel_save(XLWorkbook _workbook, string path)
        {
            _workbook.SaveAs(path);
        }
        public void save_measure(List<com_port.meas_string> reference, string path)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(excel_file_name);

            worksheet.Cell(my_headings.throttle.column    + 1).Value        = my_headings.throttle.Name;
            worksheet.Cell(my_headings.turns.column       + 1).Value        = my_headings.turns.Name;
            worksheet.Cell(my_headings.Thrust.column      + 1).Value        = my_headings.Thrust.Name;
            worksheet.Cell(my_headings.vibration.column   + 1).Value        = my_headings.vibration.Name;
            worksheet.Cell(my_headings.Volt.column        + 1).Value        = my_headings.Volt.Name;
            worksheet.Cell(my_headings.gr_W.column        + 1).Value        = my_headings.gr_W.Name;
            worksheet.Cell(my_headings.Amp.column         + 1).Value        = my_headings.Amp.Name;
            worksheet.Cell(my_headings.Speed.column       + 1).Value        = my_headings.Speed.Name;

            worksheet.Cell(my_headings.meas_number.column + 1).Value      = my_headings.meas_number.Name;
            worksheet.Cell(my_headings.meas_number.column + 2).Value      = reference.Count;

            int count = 2;
            foreach(com_port.meas_string obj in reference)
            {
                worksheet.Cell(my_headings.throttle.column  +  count).Value = obj.throttle;
                worksheet.Cell(my_headings.turns.column     +  count).Value = obj.turns;
                worksheet.Cell(my_headings.Thrust.column    +  count).Value = obj.Thrust;
                worksheet.Cell(my_headings.vibration.column +  count).Value = obj.vibration;
                worksheet.Cell(my_headings.Volt.column      +  count).Value = obj.Volt;
                worksheet.Cell(my_headings.gr_W.column      +  count).Value = obj.gr_W;
                worksheet.Cell(my_headings.Amp.column       +  count).Value = obj.Amp;
                worksheet.Cell(my_headings.Speed.column     + count).Value  = obj.speed;
                count++;
            }

            excel_save(workbook, path);
        }
        public void load_file(ref List<com_port.meas_string> reference, string path)
        {
            var workbook = new XLWorkbook(path);
            var worksheet = workbook.Worksheet(excel_file_name);

            var rows = worksheet.RangeUsed().RowsUsed(); // Skip header row

            int meas_number = get_meas_number(workbook);
            if(meas_number == 0) // если нет измерений или файл не поддерживается
            {

            }

            int offset = 2;

            for (int i = offset; i < meas_number + offset; i ++)
            {
                com_port.meas_string this_measure = new com_port.meas_string(); // ListViewItem(worksheet.Cell("A" + i).GetValue<string>());

                // item.SubItems.Add( worksheet.Cell("A" + i).GetValue<string>());
                this_measure.throttle   =  worksheet.Cell(my_headings.throttle.column  + i).GetValue<double>();
                this_measure.turns      =  worksheet.Cell(my_headings.turns.column     + i).GetValue<double>();
                this_measure.Thrust     =  worksheet.Cell(my_headings.Thrust.column    + i).GetValue<double>();
                this_measure.vibration  =  worksheet.Cell(my_headings.vibration.column + i).GetValue<double>();
                this_measure.Volt       =  worksheet.Cell(my_headings.Volt.column      + i).GetValue<double>();
                this_measure.gr_W       =  worksheet.Cell(my_headings.gr_W.column      + i).GetValue<double>();
                this_measure.Amp        =  worksheet.Cell(my_headings.Amp.column       + i).GetValue<double>();
                this_measure.speed      =  worksheet.Cell(my_headings.Speed.column     + i).GetValue<double>();

                reference.Add(this_measure);
            }

        }
        int get_meas_number(XLWorkbook workbook)
        {
            int result = 0;
            var worksheet = workbook.Worksheet(excel_file_name);
            if (worksheet.Cell(my_headings.meas_number.column + 1).GetValue<string>() == my_headings.meas_number.Name)
                return worksheet.Cell(my_headings.meas_number.column + 2).GetValue<int>();
            else
                return result;
        }


    }
}
