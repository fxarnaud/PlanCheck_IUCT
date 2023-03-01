using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;

namespace PlanCheck
{
    public class Comparator
    {
        public (string, SolidColorBrush) CompareDatas(string expectedV, string measuredV, string comparator)
        {
            //METTRE ICI LES AUTRES CAS POSSIBLES SI ON A DES VARIATIONS OU SI C'EST JUSTE INFO

            Result_Status result = new Result_Status();
            (string, SolidColorBrush) res = result.INFO;

            if (comparator == "=")  //equality condition for string and numbers
            {
                res = (measuredV == expectedV) ? result.True : result.False;

            }
            else
            {
                // > or < only for numbers. not for strings
                if ((float.TryParse(expectedV, out float expectedV_number) is true) && (float.TryParse(measuredV, out float measuredV_number)) is true)
                {
                    if (comparator == ">")
                    {
                        res = (measuredV_number > expectedV_number) ? result.True : result.False;
                    }
                    if (comparator == "<")
                    {
                        res = (measuredV_number < expectedV_number) ? result.True : result.False;
                    }
                }
                else
                {
                    res = result.False;
                }
            }
            return res;
        }
    }
}
