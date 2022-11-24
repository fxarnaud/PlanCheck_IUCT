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
        private string _protocolName;
        private double _CTslicewidth;
        private string _algoName;
        private double _gridsize;

        private List<string> _optionComp = new List<string>();

        private String _prescriptionPercentage;
        private String _normalisationMode;

        
        public read_check_protocol(string pathToProtocolCheck)  //Constructor
        {


            #region open xls file, get the cells
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
            #endregion

            _protocolName = xlRange1.Cells[1, 2].Value2;
            _CTslicewidth = xlRange1.Cells[2, 2].Value2;
            _algoName = xlRange1.Cells[3, 2].Value2;
            
            
            int optnumber = 3;
            //string[] optionComp = null;
            String tempo1;
            String tempo2;
            while (xlRange1.Cells[3, optnumber].Text != "") // parse the excel line from col 3 to first empty cell
            {
                tempo1 = xlRange1.Cells[3, optnumber].Text;
                tempo2 = tempo1.Replace(',','.');// replace , by .
                _optionComp.Add(tempo2);
                
                optnumber++;
            }

            _gridsize = xlRange1.Cells[4, 2].Value2;

            _prescriptionPercentage = xlRange1.Cells[5, 2].Text;
            _normalisationMode = xlRange1.Cells[6, 2].Text;



            #region Exemple de lecture de cellules 1
            //excel is not zero based!!
            //MessageBox.Show(xlRange1.Cells[2, 2].Value2.ToString() + "\t");
            //MessageBox.Show(xlRange2.Cells[2, 2].Value2.ToString() + "\t");
            #endregion

            #region Exemple de lecture de cellules 2
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
            #endregion

            #region cleanup excel
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
            #endregion

        }
        public double CTslicewidth
        {
            get { return _CTslicewidth; }
        }
        public string protocolName
        {
            get { return _protocolName; }
        }
        public string algoName
        {
            get { return _algoName; }
        }
        public double gridSize
        {
            get { return _gridsize; }
        }
        public List<string> optionComp
        {
            get { return _optionComp; }
        }
        public string prescriptionPercentage
        {
            get { return _prescriptionPercentage; }
        }
        public string normalisationMode
        {
            get { return _normalisationMode; }
        }
    }
}
