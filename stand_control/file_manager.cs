using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Windows.Forms;


namespace file_manager
{
    public class Excel
    {
        
        public Excel()
        {

        }
        void excel_save(XLWorkbook _workbook, string path)
        {
            _workbook.SaveAs(path);
        }
        public void save_measure(ListView this_listView, string path)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("результаты испытаний");

            worksheet.Cell("A" + 1).Value = this_listView.Columns[0].Text;
            worksheet.Cell("B" + 1).Value = this_listView.Columns[1].Text;
            worksheet.Cell("C" + 1).Value = this_listView.Columns[2].Text;
            worksheet.Cell("D" + 1).Value = this_listView.Columns[3].Text;
            worksheet.Cell("E" + 1).Value = this_listView.Columns[4].Text;
            worksheet.Cell("F" + 1).Value = this_listView.Columns[5].Text;

            int count = 2;
            foreach(ListViewItem obj in this_listView.Items)
            {
                worksheet.Cell("A" +  count).Value = obj.SubItems[0].Text;
                worksheet.Cell("B" +  count).Value = obj.SubItems[1].Text;
                worksheet.Cell("C" +  count).Value = obj.SubItems[2].Text;
                worksheet.Cell("D" +  count).Value = obj.SubItems[3].Text;
                worksheet.Cell("E" +  count).Value = obj.SubItems[4].Text;
                worksheet.Cell("F" +  count).Value = obj.SubItems[5].Text;
                count++;
            }

            excel_save(workbook, path);
        }
        public void load_file(ref ListView this_listView, string path)
        {
            var workbook = new XLWorkbook(path);
            var worksheet = workbook.Worksheet("результаты испытаний");

            var rows = worksheet.RangeUsed().RowsUsed(); // Skip header row

            for (int i = 2; i <14; i ++)
            {
                ListViewItem item = new ListViewItem(worksheet.Cell("A" + i).GetValue<string>());
           
               // item.SubItems.Add( worksheet.Cell("A" + i).GetValue<string>());
                item.SubItems.Add( worksheet.Cell("B" + i).GetValue<string>());
                item.SubItems.Add( worksheet.Cell("C" + i).GetValue<string>());
                item.SubItems.Add( worksheet.Cell("D" + i).GetValue<string>());
                item.SubItems.Add( worksheet.Cell("E" + i).GetValue<string>());
                item.SubItems.Add( worksheet.Cell("F" + i).GetValue<string>());
           
                this_listView.Items.Add(item);
            }

        }


    }
}
