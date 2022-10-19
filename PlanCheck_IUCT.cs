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
            if (context == null) 
                MessageBox.Show("Merci de charger un plan");
            var planSetup = context.PlanSetup;
            if (planSetup == null) 
                MessageBox.Show("Merci de charger un plan");
            Perform(planSetup, context);
        }

        public static void Perform(PlanSetup planSetup, ScriptContext context)
        {

            //Get Plan information
            PreliminaryInformation pinfo = new PreliminaryInformation(context);

            //Generate Main Window
            var window = new MainWindow(planSetup, pinfo,context); //passer pinfo dans main window
            


            //First point check    
            Check_Algorithm c_algo = new Check_Algorithm(pinfo,context);
            var check_point1 = new CheckScreen_Global(c_algo.Title, c_algo.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point1);



            Check_Course c_course = new Check_Course(pinfo, context);
            var check_point2 = new CheckScreen_Global(c_course.Title, c_course.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point2);


            Check_CT c_CT = new Check_CT(pinfo, context);
            var check_point3 = new CheckScreen_Global(c_CT.Title, c_CT.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            window.AddCheck(check_point3);

            //Put here other class tests. Must be the same as Check_Algorithm class
            ////

            window.ShowDialog();
        }
    }
}
