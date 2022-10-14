using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanCheck_IUCT.Users
{
    public class IUCT_Users
    {
        private List<IUCT_User> _users_list;
        public IUCT_Users()
        {
            _users_list = new List<IUCT_User>();

            IUCT_User carillo = new IUCT_User() { UserFirstName = "Fabienne", UserFamilyName = "Carillo", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = System.Windows.Media.Brushes.PapayaWhip, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(carillo);

            IUCT_User arnaud = new IUCT_User() { UserFirstName = "FXavier", UserFamilyName = "Arnaud", Gender = "H",  Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.ForestGreen, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(arnaud);

            IUCT_User undefined = new IUCT_User() { UserFirstName = "indefini", UserFamilyName = "indefini",Gender = "F", Function = "indefini", UserBackgroundColor = System.Windows.Media.Brushes.Aqua, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(undefined);
        }  
        

        public List<IUCT_User> UsersList
        {
            get { return _users_list; }
            set { _users_list = value; }
        }

    }
}
