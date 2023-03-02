using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanCheck
{
    public class NTO
    {
        public double priority;
        public string mode;
        public double falloff;
        public double startPercentageDose;
        public double stopPercentageDose;
        public double distanceTotTarget;


        public NTO(string _mode,double _priority, double _falloff, double _start, double _stop, double _distance)
        {

            mode = _mode;
            priority = _priority;
            falloff = _falloff;
            startPercentageDose = _start;
            stopPercentageDose = _stop;
            distanceTotTarget = _distance;  

        
        }

    }
}
