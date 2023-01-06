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
using System.IO;
using PlanCheck_IUCT.Users;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Navigation;
using Excel = Microsoft.Office.Interop.Excel;
// Do "Add reference" in reference manager --> COM tab --> Microsoft Excel 16 object...

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

            #region check if a plan with dose is loaded, no verification plan allowed

            if (context == null)
            {
                MessageBox.Show("Merci de charger un patient et un plan");
                return;
            }

            if (context.PlanSetup == null)
            {
                MessageBox.Show("Merci de charger un plan");
                return;
            }
            if (context.PlanSetup.PlanIntent == "VERIFICATION")
            {
                MessageBox.Show("Merci de charger un plan qui ne soit pas un plan de v�rification");
                return;
            }
            if (!context.PlanSetup.IsDoseValid)
            {
                MessageBox.Show("Merci de charger un plan avec une dose");
                return;
            }

            if (context.PlanSetup.RTPrescription == null)
                MessageBox.Show("Ce plan n'est li� � aucune prescription"); // run anyway even if there is no prescription
            #endregion

            
            //get the full location of the assembly 
            string fullPath = Assembly.GetExecutingAssembly().Location;
            //get the folder that's in
            string theDirectory = Path.GetDirectoryName(fullPath);
             // set current directory as the .dll directory
            Directory.SetCurrentDirectory(theDirectory);
            // hardcoded dir           Directory.SetCurrentDirectory(@"\\srv015\SF_COM\SIMON_LU\scriptsEclipse\00004-plancheck\Plancheck");
           

            Perform(context);
        }


        public static void Perform(ScriptContext context)
        {
            var planSetup = context.PlanSetup;
            PreliminaryInformation pinfo = new PreliminaryInformation(context);    //Get Plan information...      
            var window = new MainWindow(pinfo, context); //passer pinfo dans main window ...
            window.ShowDialog(); /// AFFICHE LA FENETRE
        }
    }
}
