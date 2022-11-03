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


            #region planners   
            #region color theme 1
            /*
            IUCT_User carillo = new IUCT_User() { UserFirstName = "Fabienne", UserFamilyName = "Carillo", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = System.Windows.Media.Brushes.PapayaWhip, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(carillo);
            IUCT_User recordon = new IUCT_User() { UserFirstName = "Frédérique", UserFamilyName = "Recordon", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = System.Windows.Media.Brushes.Salmon, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(recordon);
            IUCT_User lacaze = new IUCT_User() { UserFirstName = "Thierry", UserFamilyName = "Lacaze", Gender = "H", Function = "Dosimetriste", UserBackgroundColor = System.Windows.Media.Brushes.Yellow, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(lacaze);
            IUCT_User defour = new IUCT_User() { UserFirstName = "Nathalie", UserFamilyName = "Defour", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = System.Windows.Media.Brushes.AliceBlue, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(defour);
            IUCT_User lanaspeze = new IUCT_User() { UserFirstName = "Christel", UserFamilyName = "Lanaspeze", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = System.Windows.Media.Brushes.Firebrick, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(lanaspeze);
            IUCT_User cavet = new IUCT_User() { UserFirstName = "Clémence", UserFamilyName = "Cavet", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = System.Windows.Media.Brushes.Red, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(cavet);
            */
            #endregion
            #region color theme 2
            System.Windows.Media.SolidColorBrush tdmBGcolor1 = System.Windows.Media.Brushes.AntiqueWhite;
            System.Windows.Media.SolidColorBrush tdmFGcolor1 = System.Windows.Media.Brushes.MidnightBlue;
            IUCT_User carillo = new IUCT_User() { UserFirstName = "Fabienne", UserFamilyName = "Carillo", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = tdmBGcolor1, UserForeGroundColor = tdmFGcolor1 };
            _users_list.Add(carillo);
            IUCT_User recordon = new IUCT_User() { UserFirstName = "Frédérique", UserFamilyName = "Recordon", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = tdmBGcolor1, UserForeGroundColor = tdmFGcolor1 };
            _users_list.Add(recordon);
            IUCT_User lacaze = new IUCT_User() { UserFirstName = "Thierry", UserFamilyName = "Lacaze", Gender = "H", Function = "Dosimetriste", UserBackgroundColor = tdmBGcolor1, UserForeGroundColor = tdmFGcolor1 };
            _users_list.Add(lacaze);
            IUCT_User defour = new IUCT_User() { UserFirstName = "Nathalie", UserFamilyName = "Defour", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = tdmBGcolor1, UserForeGroundColor = tdmFGcolor1 };
            _users_list.Add(defour);
            IUCT_User lanaspeze = new IUCT_User() { UserFirstName = "Christel", UserFamilyName = "Lanaspeze", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = tdmBGcolor1, UserForeGroundColor = tdmFGcolor1 };
            _users_list.Add(lanaspeze);
            IUCT_User cavet = new IUCT_User() { UserFirstName = "Clémence", UserFamilyName = "Cavet", Gender = "F", Function = "Dosimetriste", UserBackgroundColor = tdmBGcolor1, UserForeGroundColor = tdmFGcolor1 };
            _users_list.Add(cavet);
            #endregion
            #endregion

            #region physicists
            #region color scheme 1 whith background
            /*
            IUCT_User arnaud = new IUCT_User() { UserFirstName = "FXavier", UserFamilyName = "Arnaud", Gender = "H",  Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.ForestGreen, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(arnaud);

            IUCT_User simon = new IUCT_User() { UserFirstName = "Luc", UserFamilyName = "Simon", Gender = "H", Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.DeepSkyBlue, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(simon);

            IUCT_User graulieres = new IUCT_User() { UserFirstName = "Eliane", UserFamilyName = "Graulieres", Gender = "F", Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.DeepPink, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(graulieres);

            IUCT_User hangard = new IUCT_User() { UserFirstName = "Gregory", UserFamilyName = "Hangard", Gender = "H", Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.Gold, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(hangard);

            IUCT_User parent = new IUCT_User() { UserFirstName = "Laure", UserFamilyName = "Parent", Gender = "F", Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.Yellow, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(parent);

            IUCT_User brun = new IUCT_User() { UserFirstName = "Thomas", UserFamilyName = "Brun", Gender = "H", Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.RosyBrown, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(brun);

            IUCT_User vieillevigne = new IUCT_User() { UserFirstName = "Laure", UserFamilyName = "Vieillevigne", Gender = "F", Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.LightGreen, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(vieillevigne);

            IUCT_User stadler = new IUCT_User() { UserFirstName = "Marine", UserFamilyName = "Stadler", Gender = "F", Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.Aquamarine, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(stadler);

            IUCT_User tournier = new IUCT_User() { UserFirstName = "Aurélie", UserFamilyName = "Tournier", Gender = "F", Function = "Physicien", UserBackgroundColor = System.Windows.Media.Brushes.DarkTurquoise, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(tournier);
            */
            #endregion

            #region color scheme 2

            System.Windows.Media.SolidColorBrush phyBGcolor1 = System.Windows.Media.Brushes.AntiqueWhite;
            System.Windows.Media.SolidColorBrush phyFGcolor1 = System.Windows.Media.Brushes.MidnightBlue;

            IUCT_User arnaud = new IUCT_User() { UserFirstName = "FXavier", UserFamilyName = "Arnaud", Gender = "H", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(arnaud);

            IUCT_User simon = new IUCT_User() { UserFirstName = "Luc", UserFamilyName = "Simon", Gender = "H", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(simon);

            IUCT_User graulieres = new IUCT_User() { UserFirstName = "Eliane", UserFamilyName = "Graulieres", Gender = "F", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(graulieres);

            IUCT_User hangard = new IUCT_User() { UserFirstName = "Gregory", UserFamilyName = "Hangard", Gender = "H", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(hangard);

            IUCT_User parent = new IUCT_User() { UserFirstName = "Laure", UserFamilyName = "Parent", Gender = "F", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(parent);

            IUCT_User brun = new IUCT_User() { UserFirstName = "Thomas", UserFamilyName = "Brun", Gender = "H", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(brun);

            IUCT_User vieillevigne = new IUCT_User() { UserFirstName = "Laure", UserFamilyName = "Vieillevigne", Gender = "F", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(vieillevigne);

            IUCT_User stadler = new IUCT_User() { UserFirstName = "Marine", UserFamilyName = "Stadler", Gender = "F", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(stadler);

            IUCT_User tournier = new IUCT_User() { UserFirstName = "Aurélie", UserFamilyName = "Tournier", Gender = "F", Function = "Physicien", UserBackgroundColor = phyBGcolor1, UserForeGroundColor = phyFGcolor1 };
            _users_list.Add(tournier);
            #endregion
            #endregion

            #region oncologists 

            #region color scheme 1
            /*
                         IUCT_User attal = new IUCT_User() { UserFirstName = "Justine", UserFamilyName = "Attal", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.LightPink, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(attal);

            IUCT_User chira = new IUCT_User() { UserFirstName = "Ciprian", UserFamilyName = "Chira", Gender = "H", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.Orange, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(chira);

            IUCT_User couarde = new IUCT_User() { UserFirstName = "Laetitia", UserFamilyName = "Couarde", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.DeepPink, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(couarde);

            IUCT_User dalmasso = new IUCT_User() { UserFirstName = "Céline", UserFamilyName = "Dalmasso", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.IndianRed, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(dalmasso);
            
            IUCT_User desrousseaux = new IUCT_User() { UserFirstName = "Desrousseaux", UserFamilyName = "Jacques", Gender = "H", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.Orange, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(desrousseaux);

            IUCT_User ducassou = new IUCT_User() { UserFirstName = "Anne", UserFamilyName = "Ducassou", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.HotPink, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(ducassou);

            IUCT_User izar = new IUCT_User() { UserFirstName = "Françoise", UserFamilyName = "Izar", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.Yellow, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(izar);

            IUCT_User keller = new IUCT_User() { UserFirstName = "Audrey", UserFamilyName = "Keller", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.BlueViolet, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(keller);

            IUCT_User khalifa = new IUCT_User() { UserFirstName = "Jonathan", UserFamilyName = "Khalifa", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.Gold, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(khalifa); 

            IUCT_User laprie = new IUCT_User() { UserFirstName = "Anne", UserFamilyName = "Laprie", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.MediumPurple, UserForeGroundColor = System.Windows.Media.Brushes.Navy };
            _users_list.Add(laprie);

            IUCT_User massabeau = new IUCT_User() { UserFirstName = "Carole", UserFamilyName = "Massabeau", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.BlueViolet, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(massabeau);

            IUCT_User modesto = new IUCT_User() { UserFirstName = "Anouchka", UserFamilyName = "Modesto", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.GreenYellow, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(modesto);

            IUCT_User moyal = new IUCT_User() { UserFirstName = "Elizabeth", UserFamilyName = "Moyal", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.Red, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(moyal);

            IUCT_User piram = new IUCT_User() { UserFirstName = "Lucie", UserFamilyName = "Piram", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.LightBlue, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(piram);

            IUCT_User pouedras = new IUCT_User() { UserFirstName = "Juliette", UserFamilyName = "Pouedras", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.DarkTurquoise, UserForeGroundColor = System.Windows.Media.Brushes.Black };
            _users_list.Add(pouedras);

            IUCT_User preault = new IUCT_User() { UserFirstName = "Mickael", UserFamilyName = "Preault", Gender = "H", Function = "Radiothérapeute", UserBackgroundColor = System.Windows.Media.Brushes.Azure, UserForeGroundColor = System.Windows.Media.Brushes.AntiqueWhite };
            _users_list.Add(preault);


             */
            #endregion

            #region color scheme 2
            System.Windows.Media.SolidColorBrush doctorBGcolor1 = System.Windows.Media.Brushes.Wheat;
            System.Windows.Media.SolidColorBrush doctorFGcolor1 = System.Windows.Media.Brushes.Black;

            IUCT_User attal = new IUCT_User() { UserFirstName = "Justine", UserFamilyName = "Attal", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(attal);

            IUCT_User chira = new IUCT_User() { UserFirstName = "Ciprian", UserFamilyName = "Chira", Gender = "H", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(chira);

            IUCT_User couarde = new IUCT_User() { UserFirstName = "Laetitia", UserFamilyName = "Couarde", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(couarde);

            IUCT_User dalmasso = new IUCT_User() { UserFirstName = "Céline", UserFamilyName = "Dalmasso", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(dalmasso);
            
            IUCT_User desrousseaux = new IUCT_User() { UserFirstName = "Desrousseaux", UserFamilyName = "Jacques", Gender = "H", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(desrousseaux);

            IUCT_User ducassou = new IUCT_User() { UserFirstName = "Anne", UserFamilyName = "Ducassou", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(ducassou);

            IUCT_User izar = new IUCT_User() { UserFirstName = "Françoise", UserFamilyName = "Izar", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(izar);

            IUCT_User keller = new IUCT_User() { UserFirstName = "Audrey", UserFamilyName = "Keller", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(keller);

            IUCT_User khalifa = new IUCT_User() { UserFirstName = "Jonathan", UserFamilyName = "Khalifa", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(khalifa); 

            IUCT_User laprie = new IUCT_User() { UserFirstName = "Anne", UserFamilyName = "Laprie", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(laprie);

            IUCT_User massabeau = new IUCT_User() { UserFirstName = "Carole", UserFamilyName = "Massabeau", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(massabeau);

            IUCT_User modesto = new IUCT_User() { UserFirstName = "Anouchka", UserFamilyName = "Modesto", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(modesto);

            IUCT_User moyal = new IUCT_User() { UserFirstName = "Elizabeth", UserFamilyName = "Moyal", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(moyal);

            IUCT_User piram = new IUCT_User() { UserFirstName = "Lucie", UserFamilyName = "Piram", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(piram);

            IUCT_User pouedras = new IUCT_User() { UserFirstName = "Juliette", UserFamilyName = "Pouedras", Gender = "F", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(pouedras);

            IUCT_User preault = new IUCT_User() { UserFirstName = "Mickael", UserFamilyName = "Preault", Gender = "H", Function = "Radiothérapeute", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(preault);

            IUCT_User undefined = new IUCT_User() { UserFirstName = "indefini", UserFamilyName = "indefini", Gender = "F", Function = "indefini", UserBackgroundColor = doctorBGcolor1, UserForeGroundColor = doctorFGcolor1 };
            _users_list.Add(undefined);
            #endregion

            #endregion



        }


        public List<IUCT_User> UsersList
        {
            get { return _users_list; }
            set { _users_list = value; }
        }

    }
}
