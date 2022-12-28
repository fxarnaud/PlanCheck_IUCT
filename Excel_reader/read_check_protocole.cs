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
        private String _enableGating;
        private List<Tuple<string, double>> _couchStructures = new List<Tuple<string, double>>();
        private List<Tuple<string, double, double, double>> _clinicalStructures = new List<Tuple<string, double, double, double>>();
        private List<Tuple<string, double>> _optStructures = new List<Tuple<string, double>>();

        public read_check_protocol(string pathToProtocolCheck)  //Constructor
        {


            #region open xls file, get the cells
            // open excel
            Excel.Application xlApp = new Excel.Application();

            // open file
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(pathToProtocolCheck, ReadOnly: true);

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

            // open the sheet 4
            Excel._Worksheet xlWorksheet4 = xlWorkbook.Sheets[4];
            // get the cells 4
            Excel.Range xlRange4 = xlWorksheet4.UsedRange;
            #endregion

            #region feuille 1 General
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
                tempo2 = tempo1.Replace(',', '.');// replace , by .
                _optionComp.Add(tempo2);

                optnumber++;
            }

            _gridsize = xlRange1.Cells[4, 2].Value2;

            _prescriptionPercentage = xlRange1.Cells[5, 2].Text;
            _normalisationMode = xlRange1.Cells[6, 2].Text;

            _enableGating = xlRange1.Cells[7, 2].Text;
            #endregion
            //nt line = 0;

            #region feuille 2 clinical structures

            int nRowsClinicalStruct = xlRange2.Rows.Count;
            string oneClinicalStruct = null;
            double huCS = 0;
            for (int i = 2; i <= nRowsClinicalStruct; i++) // read all lines sheet 2
            {
                var temp1 = xlRange2.Cells[i, 1].Value2; // column 1
                var temp2 = xlRange2.Cells[i, 2].Value2; // column 2
                var temp3 = xlRange2.Cells[i, 3].Value2; // column 2
                var temp4 = xlRange2.Cells[i, 4].Value2; // column 2

                oneClinicalStruct = temp1.ToString();
                if (temp2 != null)
                    huCS = (double)(temp2);
                else
                    huCS = 9999;///9999 if no asssigned HU

                double volmin = 0.0;
                double volmax = 0.0;
                if ((temp3 != null) && (temp4 != null))
                {
                     volmin = (double)(temp3);
                     volmax = (double)(temp4);
                }
                else
                {
                     volmin = 9999;
                     volmax = 9999;
                }
                Tuple<string, double, double, double> aCSElement = new Tuple<string, double, double, double>(oneClinicalStruct, huCS, volmin, volmax);
                _clinicalStructures.Add(aCSElement);
            }
            #endregion


            #region feuille 3 opt structures

            int nRowsOptlStruct = xlRange3.Rows.Count;
            string oneOptStruct = null;
            double huOS = 0;
            for (int i = 2; i <= nRowsOptlStruct; i++) // read all lines sheet 2
            {
                var temp1 = xlRange3.Cells[i, 1].Value2; // column 1
                var temp2 = xlRange3.Cells[i, 2].Value2; // column 2
                oneOptStruct = temp1.ToString();
                if (temp2 != null)
                    huOS = (double)(temp2);
                else
                    huOS = 9999;///9999 if no asssigned HU
                Tuple<string, double> aOSElement = new Tuple<string, double>(oneOptStruct, huOS);//, "cat", true);
                _optStructures.Add(aOSElement);
            }


            #endregion

            #region feuille 4 Couch structures

            int nRowsCouchStruct = xlRange4.Rows.Count;
            string couchEl = null;
            double huEl = 0;
            for (int i = 2; i <= nRowsCouchStruct; i++) // read all lines sheet 4
            {
                var temp1 = xlRange4.Cells[i, 1].Value2; // column 1
                var temp2 = xlRange4.Cells[i, 2].Value2; // column 2
                couchEl = temp1.ToString();
                huEl = (double)(temp2);
                Tuple<string, double> aCouchElement = new Tuple<string, double>(couchEl, huEl);//, "cat", true);
                _couchStructures.Add(aCouchElement);
                // MessageBox.Show(aCouchElement.Item1 + aCouchElement.Item2.ToString());
            }


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
        public string enebleGating
        {
            get { return _enableGating; }
        }

        public List<Tuple<string, double>> couchStructures
        {
            get { return _couchStructures; }
        }
        public List<Tuple<string, double,double,double>> clinicalStructures
        {
            get { return _clinicalStructures; }
        }
        public List<Tuple<string, double>> optStructures
        {
            get { return _optStructures; }
        }


    }
}
