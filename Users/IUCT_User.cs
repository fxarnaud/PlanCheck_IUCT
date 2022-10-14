using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlanCheck_IUCT
{
    public class IUCT_User
    {
        private string _userfamilyname;
        private string _userfirstname;
        private string _function;
        private string _gender;
        private SolidColorBrush _userbackgroundcolor;
        private SolidColorBrush _userforegroundcolor;


        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        public string UserFamilyName
        {
            get { return _userfamilyname; }
            set { _userfamilyname = value; }
        }

        public string UserFirstName
        {
            get { return _userfirstname; }
            set { _userfirstname = value; }
        }

        public SolidColorBrush UserBackgroundColor
        {
            get { return _userbackgroundcolor; }
            set { _userbackgroundcolor = value; }
        }
        public string Function
        {
            get { return _function; }
            set { _function = value; }
        }

        public SolidColorBrush UserForeGroundColor
        {
            get { return _userforegroundcolor; }
            set { _userforegroundcolor = value; }
        }

    }
}
