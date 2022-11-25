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

            #region check if a plan with dose is loaded
            bool validPlanWithDoseisLoaded = true;
            if(context == null)
            {
               MessageBox.Show("Merci de charger un plan");
               validPlanWithDoseisLoaded = false;
            }
           
            if(context.PlanSetup == null)
            {
                MessageBox.Show("Merci de charger un plan");
                validPlanWithDoseisLoaded = false;
            }
            if(!context.PlanSetup.IsDoseValid)
            {
                MessageBox.Show("Merci de charger un plan avec une dose");
                validPlanWithDoseisLoaded = false;
            }

            if (context.PlanSetup.RTPrescription == null)
                MessageBox.Show("Ce plan n'est lié à aucune prescription"); // run anyway even if there is no prescription
            #endregion

            
            if (validPlanWithDoseisLoaded)
                Perform(context);
       
        }
        
        public static void Perform(ScriptContext context)
        {
            
            var planSetup = context.PlanSetup;
            
            PreliminaryInformation pinfo = new PreliminaryInformation(context);    //Get Plan information...      

            var window = new MainWindow(pinfo,context); //passer pinfo dans main window ...

            window.ShowDialog(); /// AFFICHE LA FENETRE



            //string pathpath = @"\\srv015\SF_COM\ARNAUD_FX\ECLIPSE_SCRIPTING\Plan_Check_new\check_protocole\protocole-prostate.xlsx";
            //read_check_protocol rcp = new read_check_protocol(pathpath);

            // marche pas: 
            // read_check_protocol rcp = new read_check_protocol(window.myFullFilename);
            


            #region Liste des checks
            /*
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
            */
            #endregion

            //window.ShowDialog(); /// AFFICHE LA FENETRE

        }
    }
}
