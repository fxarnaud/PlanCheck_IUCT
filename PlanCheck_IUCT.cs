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
// Do "Add reference" in reference manager --> COM tab --> Microsoft Excel 16 object

[assembly: AssemblyVersion("1.0.0.1")]
namespace VMS.TPS
{
    public class Script
    {
        // this is a test
        public Script()
        {
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context)
        {
            bool validPlanWithDoseisLoaded = true;

            if(context == null)
            {
               MessageBox.Show("Merci de charger un plan");
               validPlanWithDoseisLoaded = false;
            }

            var planSetup = context.PlanSetup;
            if(planSetup == null)
            {
                MessageBox.Show("Merci de charger un plan");
                validPlanWithDoseisLoaded = false;
            }
            if(!planSetup.IsDoseValid)
            {
                MessageBox.Show("Merci de charger un plan avec une dose");
                validPlanWithDoseisLoaded = false;
            }

            if (planSetup.RTPrescription == null)
                MessageBox.Show("Ce plan n'est lié à aucune prescription"); // run anyway even if there is no prescription

//            throw new ApplicationException("Please load an external beam plan that will be verified.");
            if(validPlanWithDoseisLoaded)
                Perform(planSetup, context);
       
        }
        
        public static void Perform(PlanSetup planSetup, ScriptContext context)
        {
            // 

            //Get Plan information...
            PreliminaryInformation pinfo = new PreliminaryInformation(context);
           
            //Generate Main Window
            var window = new MainWindow(planSetup, pinfo,context); //passer pinfo dans main window ...


            string pathpath = @"\\srv015\SF_COM\ARNAUD_FX\ECLIPSE_SCRIPTING\Plan_Check_new\check_protocole\protocole-prostate.xlsx";
            read_check_protocol rcp = new read_check_protocol(pathpath);

            // marche pas: 
            // read_check_protocol rcp = new read_check_protocol(window.myFullFilename);


            #region exemple fx

            /*
             * bool _DebugMode = false;  //Modify this to debug mode

            var directorypath = @"\\srv015\radiotherapie\SCRIPTS_ECLIPSE\Opt_Structures\";
            //var directorypath = @"\\srv015\SF_COM\LACAZE T\Opt_Structures\";

            Patient mypatient = context.Patient;
            mypatient.BeginModifications();   //Mandatory to write in DataBase
            
            //   Check if a patient and a structure set is loaded
            if (context.Patient == null || context.StructureSet == null)
            {
                MessageBox.Show("Please load a patient, 3D image, and structure set before running this script.", "Opt_strucutres", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            //Let the user choose csv file using a browser
            string file;

            if (!System.IO.Directory.Exists(directorypath))
            {
                MessageBox.Show(String.Format("The default template file directory {0} defined by the script does not exist", directorypath));
                return;
            }
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.DefaultExt = "csv";
            fileDialog.InitialDirectory = directorypath;
            fileDialog.Multiselect = false;
            fileDialog.Title = "Selection du template a appliquer";
            fileDialog.ShowReadOnly = true;
            fileDialog.Filter = "CSV files (*.csv)|*.csv";
            fileDialog.FilterIndex = 0;
            fileDialog.CheckFileExists = true;
            if (fileDialog.ShowDialog() == false)
            {
                return;    // user canceled
            }
            file = fileDialog.FileName;

            if (!System.IO.File.Exists(file))
            {
                MessageBox.Show(string.Format("The template file '{0}' chosen does not exist.", file));
                return;
            }         
            
            //Reading protocol instructions and detect each bloc 
            Protocol_Datas protocol_structures = new Protocol_Datas(file, context.StructureSet);
             */
            #endregion

            Check_Course c_course = new Check_Course(pinfo, context);
            var check_point1 = new CheckScreen_Global(c_course.Title, c_course.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point1);
            
            Check_CT c_CT = new Check_CT(pinfo, context, rcp);
            var check_point2 = new CheckScreen_Global(c_CT.Title, c_CT.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point2);

 
            if (planSetup.RTPrescription != null)
            {
                Check_Prescription c_prescri = new Check_Prescription(pinfo, context,rcp);
                var check_point3 = new CheckScreen_Global(c_prescri.Title, c_prescri.Result);
                window.AddCheck(check_point3);           
            }

            Check_Plan c_algo = new Check_Plan(pinfo,context,rcp);            
            var check_point4 = new CheckScreen_Global(c_algo.Title, c_algo.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point4);
            

            Check_UM c_UM = new Check_UM(pinfo, context);
            var check_point5 = new CheckScreen_Global(c_UM.Title, c_UM.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point5);
            
  
            Check_Isocenter c_Isocenter = new Check_Isocenter(pinfo, context);
            var check_point6 = new CheckScreen_Global(c_Isocenter.Title, c_Isocenter.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point6);

            
            Check_contours c_Contours = new Check_contours(pinfo, context);
            var check_point7 = new CheckScreen_Global(c_Contours.Title, c_Contours.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point7);
            
            Check_beams c_Beams = new Check_beams(pinfo, context,rcp);
            var check_point8 = new CheckScreen_Global(c_Beams.Title, c_Beams.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point8);
            

            Check_doseDistribution c_doseDistribution = new Check_doseDistribution(pinfo, context);
            var check_point9 = new CheckScreen_Global(c_doseDistribution.Title, c_doseDistribution.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point9);

            Check_finalisation c_Finalisation = new Check_finalisation(pinfo, context);
            var check_point10 = new CheckScreen_Global(c_Finalisation.Title, c_Finalisation.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point10);
            


            //Put here other class tests. Must be the same as Check_Plan class
            ////

            window.ShowDialog();
        }
    }
}
