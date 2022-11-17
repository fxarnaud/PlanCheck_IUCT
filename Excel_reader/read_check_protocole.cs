using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Runtime.CompilerServices;
using System.Reflection;
using PlanCheck_IUCT;
using PlanCheck_IUCT.Users;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Navigation;
using Excel = Microsoft.Office.Interop.Excel;



namespace PlanCheck_IUCT
{
    public class read_check_protocol
    {
        private double _CTslicewidth;
        public read_check_protocol(string pathToProtocolCheck)  //Constructor
        {



            // open excel
            Excel.Application xlApp = new Excel.Application();
           
            // open file
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(pathToProtocolCheck);

            // open the sheet 1
            Excel._Worksheet xlWorksheet1 = xlWorkbook.Sheets[1];           
            // get the cells 1
            Excel.Range xlRange1 = xlWorksheet1.UsedRange;

            // open the sheet 2
            Excel._Worksheet xlWorksheet2 = xlWorkbook.Sheets[2];
            // get the cells 2
            Excel.Range xlRange2 = xlWorksheet2.UsedRange;

            // open the sheet 3
            Excel._Worksheet xlWorksheet3 = xlWorkbook.Sheets[3];
            // get the cells 3
            Excel.Range xlRange3 = xlWorksheet3.UsedRange;



            _CTslicewidth = xlRange1.Cells[2, 2].Value2;
            //MessageBox.Show(CTslicewidth.ToString());
            // Exemple de lecture de cellules 1
            //MessageBox.Show(xlRange1.Cells[2, 2].Value2.ToString() + "\t");
            //MessageBox.Show(xlRange2.Cells[2, 2].Value2.ToString() + "\t");

            //excel is not zero based!!


            // Exemple de lecture de cellules 2
            /*   for (int i = 1; i <= 2; i++)
            {
                for (int j = 1; j <= 2; j++)
                {
                    //new line
                    if (j == 1)
                        MessageBox.Show("\r\n");

                    //write the value to the console
                    if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                        MessageBox.Show(xlRange.Cells[i, j].Value2.ToString() + "\t");

                    //add useful things here!   
                }
            }*/
            //cleanup excel
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(xlRange1);
            Marshal.ReleaseComObject(xlRange2);
            Marshal.ReleaseComObject(xlRange3);
            Marshal.ReleaseComObject(xlWorksheet1);
            Marshal.ReleaseComObject(xlWorksheet2);
            Marshal.ReleaseComObject(xlWorksheet3);
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

        }
        public double CTslicewidth
        {
            get { return _CTslicewidth; }
        }

    }
}
