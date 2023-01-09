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
        //private List<Tuple<string, double>> _couchStructures = new List<Tuple<string, double>>();
        // private List<Tuple<string, double, double, double>> _clinicalStructures = new List<Tuple<string, double, double, double>>();
        // private List<Tuple<string, double>> _optStructures = new List<Tuple<string, double>>();

        private List<expectedStructure> _myCouchExpectedStructures = new List<expectedStructure>();
        private List<expectedStructure> _myClinicalExpectedStructures = new List<expectedStructure>();
        private List<expectedStructure> _myOptExpectedStructures = new List<expectedStructure>();


        public expectedStructure readAStructRow(Excel.Range r, int row)
        {
            expectedStructure es = new expectedStructure();
            var temp1 = r.Cells[row, 1].Value2;
            if (temp1 != null)
            {
                var temp2 = r.Cells[row, 2].Value2; // column 2
                var temp3 = r.Cells[row, 3].Value2; // column 2
                var temp4 = r.Cells[row, 4].Value2; // column 2
                var temp5 = r.Cells[row, 5].Value2; // column 2

                es.Name = (r.Cells[row, 1].Value2).ToString();
                
                if (temp2 != null)
                    es.HU = (double)(temp2);
                else
                    es.HU = 9999;
                if (temp3 != null)
                    es.volMin = (double)(temp3);
                else
                    es.volMin = 9999;
                if (temp4 != null)
                    es.volMax = (double)(temp4);
                else
                    es.volMax = 9999;
                if (temp5 != null)
                    es.expectedNumberOfPart = (int)(temp5);
                else
                    es.expectedNumberOfPart = 9999;


            }
            if (temp1 != null)
                return es;
            else
                return null;
        }

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


            #region sheet 1 General
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


            #region sheet 2 clinical structures

            int nRowsClinicalStruct = xlRange2.Rows.Count;
            int i = 0;
            for (i = 2; i <= nRowsClinicalStruct; i++) // read all lines sheet 2
            {
                expectedStructure es = readAStructRow(xlRange2, i);
                _myClinicalExpectedStructures.Add(es);

            }
            /*
            string allname=null;
            foreach (expectedStructure es1 in _myClinicalExpectedStructures)
            {
                allname += " ";
                allname += es1.Name;
            }
            MessageBox.Show(allname);   
            */
            #endregion


            #region sheet 3 opt structures

            int nRowsOptlStruct = xlRange3.Rows.Count;

            for (i = 2; i <= nRowsOptlStruct; i++) // read all lines sheet 2
            {
                expectedStructure es = readAStructRow(xlRange3, i);
                _myOptExpectedStructures.Add(es);
            }

            #endregion


            #region sheet 4 Couch structures

            int nRowsCouchStruct = xlRange4.Rows.Count;
            for (i = 2; i <= nRowsCouchStruct; i++) // read all lines sheet 4
            {
                expectedStructure es = readAStructRow(xlRange4, i);
                _myCouchExpectedStructures.Add(es);
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
        public List<expectedStructure> myClinicalExpectedStructures
        {
            get { return _myClinicalExpectedStructures; }
        }
        public List<expectedStructure> myOptExpectedStructures
        {
            get { return _myOptExpectedStructures; }
        }
        public List<expectedStructure> myCouchExpectedStructures
        {
            get { return _myCouchExpectedStructures; }
        }


    }
}
