using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Runtime.CompilerServices;
using System.Reflection;
using PlanCheck_IUCT;


[assembly: AssemblyVersion("1.0.0.1")]
namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context)
        {
            var planSetup = context.PlanSetup;
            Perform(planSetup, context);
        }

        public static void Perform(PlanSetup planSetup, ScriptContext context)
        {
            //Get Plan information
            PlanInformation pinfo = new PlanInformation(context);

            //Generate Main Window
            var window = new MainWindow(planSetup); //passer pinfo dans main window


            //First point check    
            Check_Algorithm c_algo = new Check_Algorithm(pinfo);
            var check_point1 = new CheckScreen_Global(c_algo.Title, c_algo.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point1);

            //Put here other class tests. Must be the same as Check_Algorithm class
            ////

            window.ShowDialog();
        }
    }
}
