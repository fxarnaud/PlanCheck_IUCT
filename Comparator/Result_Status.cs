using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlanCheck_IUCT
{
    public  class Result_Status
    {    
        (string, SolidColorBrush) _true = ("OK", new SolidColorBrush(Colors.LightGreen));
        (string, SolidColorBrush) _false = ("X", new SolidColorBrush(Colors.LightSalmon));
        (string, SolidColorBrush) _variation = ("WARNING", new SolidColorBrush(Colors.LightYellow));
        (string, SolidColorBrush) _informatif = ("INFORMATIF", new SolidColorBrush(Colors.Gray));



        public (string, SolidColorBrush) True
        {
            get { return _true; }
        }
        public (string, SolidColorBrush) False
        {
            get { return _false; }
        }
        public (string, SolidColorBrush) Variation
        {
            get { return _variation; }
        }
        public (string, SolidColorBrush) Informatif
        {
            get { return _informatif; }
        }
    }
}
