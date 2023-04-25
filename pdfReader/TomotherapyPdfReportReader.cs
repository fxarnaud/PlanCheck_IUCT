
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Runtime.CompilerServices;
using System.Reflection;
using PlanCheck;
using PlanCheck.Users;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Navigation;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using iText;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using System.IO;
using iText.Layout.Element;
using iText.Layout.Properties;
//using iText.Kernel.Pdf.canvas.parser.PdfTextExtractor;
//using iTextSharp;
namespace PlanCheck
{
    public class TomotherapyPdfReportReader
    {

        private tomoReportData trd;

        public void displayInfo()
        {
            String s = null;
            s += " Plan name: " + trd.planName;
            s += "\n Plan type: " + trd.planType;
            s += "\n MachineID: " + trd.machineNumber; 
            s += "\n Machine rev: " + trd.machineRevision;
            s += "\n prescriptionMode: " + trd.prescriptionMode;
            s += "\n prescriptionTotalDose: " + trd.prescriptionTotalDose;
            s += "\n prescriptionStructure: " + trd.prescriptionStructure;
            s += "\n prescriptionMode: " + trd.prescriptionMode;
            s += "\n prescriptionDosePerFraction: " + trd.prescriptionDosePerFraction;
            s += "\n prescriptionNumberOfFraction: " + trd.prescriptionNumberOfFraction;
            s += "\n approvalStatus: " + trd.approvalStatus;
            s += "\n MUplanned: " + trd.MUplanned;
            s += "\n MUplannedPerFraction: " + trd.MUplannedPerFraction;
            MessageBox.Show(s);

            s = null;
            s += "\n Field Width: " + trd.fieldWidth;

            s += "\n isDynamic: " + trd.isDynamic;

            s += "\n pitch: " + trd.pitch;
            s += "\n modulationFactor: " + trd.modulationFactor;

            


            MessageBox.Show(s);
        }
        public tomoReportData Trd { get => trd; set => trd = value; }

        public TomotherapyPdfReportReader(string pathToPdf)  //Constructor. 
        {

            trd = new tomoReportData();
            #region convert pdf 2 text file
            string outpath = Directory.GetCurrentDirectory() + @"\..\pdfReader\tomoReportData.txt";
            PdfReader pdfReader = new PdfReader(pathToPdf);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            String pageContent = null;
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                pageContent += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);

            }

            File.WriteAllText(outpath, pageContent);
            pdfDoc.Close();
            pdfReader.Close();
            #endregion

            
            System.IO.StreamReader file = new System.IO.StreamReader(Directory.GetCurrentDirectory() + @"\..\pdfReader\tomoReportData.txt");
            String line = null;
            List<string> lines = new List<string>();

            while ((line = file.ReadLine()) != null)
            {
                lines.Add(line);

            }
            MessageBox.Show(lines.Count.ToString());


            trd.planName = lines[3];
            trd.planType = lines[4];
            trd.machineNumber = lines[7];

            //string s = trd.planName + " " + trd.planType + " " + trd.machineNumber;// + " " + trd.machineRevision + "\n";
            //s += trd.prescriptionMode + " " + trd.prescriptionTotalDose;
            //MessageBox.Show(s);


            string[] separatingStrings = { "rev" };
            string[] sub1 = lines[6].Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            string[] sub2 = sub1[1].Split('/');
            trd.machineRevision = sub2[0];


            //Prescription: Median of PTV sein, 50.00 Gy

            string[] separatingStrings2 = { ": " };
            sub1 = lines[8].Split(separatingStrings2, System.StringSplitOptions.RemoveEmptyEntries); // Prescription ... Median of PTV sein, 50.00 Gy

            string[] separatingStrings3 = { ", " };
            sub2 = sub1[1].Split(separatingStrings3, System.StringSplitOptions.RemoveEmptyEntries); // Median of PTV sein ... 50.00 Gy
            string[] sub3 = sub2[1].Split(' ');                      // 
            trd.prescriptionMode = sub2[0];
            trd.prescriptionTotalDose = Convert.ToDouble(sub3[0]);


            string[] separatingStrings4 = { "of " };
            trd.prescriptionStructure = sub2[0].Split(separatingStrings4, System.StringSplitOptions.RemoveEmptyEntries)[1];
            trd.prescriptionMode = sub2[0].Split(separatingStrings4, System.StringSplitOptions.RemoveEmptyEntries)[0];



            trd.prescriptionDosePerFraction = Convert.ToDouble(lines[9].Split(separatingStrings2, System.StringSplitOptions.RemoveEmptyEntries)[1]);
            trd.prescriptionNumberOfFraction = Convert.ToInt32(lines[10].Split(separatingStrings2, System.StringSplitOptions.RemoveEmptyEntries)[1]);

            trd.approvalStatus = lines[12].Split(separatingStrings2, System.StringSplitOptions.RemoveEmptyEntries)[1];

            sub2 = lines[13].Split(':');
            sub3 = sub2[1].Split('/');
            trd.MUplanned = Convert.ToDouble(sub3[0]);
            trd.MUplannedPerFraction = Convert.ToDouble(sub3[1]);

            sub2 = lines[14].Split(':');
            sub3 = sub2[1].Split(',');
            trd.fieldWidth = Convert.ToDouble(sub3[0]);
            
            if (sub3[1].Contains("Dynamic"))
                trd.isDynamic = true;
            else
                trd.isDynamic = false;
            
            sub2 = lines[15].Split(':');
            trd.pitch = Convert.ToDouble(sub2[1]);

            sub2 = lines[16].Split(':');
            sub3 = sub2[1].Split('/');
            trd.modulationFactor = Convert.ToDouble(sub3[0]);


            trd.gantryPeriod = Convert.ToDouble(lines[19]);
            trd.gantryNumberOfRotation = Convert.ToDouble(lines[20]);

            trd.couchSpeed=Convert.ToDouble(lines[24]);
            trd.couchSpeed=Convert.ToDouble(lines[25]);
            
            sub2 = lines[26].Split(':');
            trd.redLaserXoffset = Convert.ToDouble(sub2[1]);
            
            /*
        
      
     

        public double redLaserXoffset;
        public double redLaserYoffset;
        public double redLaserZoffset;
        public double beamOnTime;
        public String deliveryMode;

        public String algorithm;
        public String resolutionCalculation;
        public double refDose;
        public double refPointX;
        public double refPointY;
        public double refPointZ;
        public String planningMethod;
        public String patientName;
        public String patientFirstName;
        public String patientID;
        public String patientDateOfBirth;
        public String patientSex;
        public String HUcurve;
        public String approverID;
        public bool isHeadFirst;
        public bool isSupine;
        public double originX;
        public double originY;
        public double originZ;
        public int numberOfCTslices;*/
        }




    }
}