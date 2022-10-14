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
using PlanCheck_IUCT.Users;

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
            PreliminaryInformation pinfo = new PreliminaryInformation(context);

            //Generate Main Window
            var window = new MainWindow(planSetup, pinfo ); //passer pinfo dans main window
            


            //First point check    
            Check_Algorithm c_algo = new Check_Algorithm(pinfo);
            var check_point1 = new CheckScreen_Global(c_algo.Title, c_algo.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point1);

            Check_Course c_course = new Check_Course(pinfo, context);
            var check_point2 = new CheckScreen_Global(c_course.Title, c_course.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point2);

            //Put here other class tests. Must be the same as Check_Algorithm class
            ////

            window.ShowDialog();
        }
    }
}
