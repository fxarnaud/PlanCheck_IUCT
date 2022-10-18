using PlanCheck_IUCT.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace PlanCheck_IUCT
{
    public class PreliminaryInformation
    {
        private ScriptContext _ctx;
        private string _patientname;
        private string _patientdob;
        private DateTime _patientdob_dt;
        private string _coursename;
        private string _planname;
        private IUCT_User _plancreator;
        private IUCT_User _currentuser;
        private IUCT_User _doctor;
        private string _algoname;
        private string _mlctype;

        public PreliminaryInformation(ScriptContext ctx)  //Constructor
        {
            _ctx = ctx;

            _patientname = ctx.Patient.Name;
            _patientdob_dt = (DateTime)ctx.Patient.DateOfBirth;
            _patientdob = _patientdob_dt.Day+"/"+ _patientdob_dt.Month + "/" + _patientdob_dt.Year;
            _coursename = ctx.Course.Id;
            _planname = ctx.PlanSetup.Id;
            _plancreator = GetUser("creator");
            _currentuser = GetUser("currentuser");
            _doctor = GetUser("doctor");
            _algoname = ctx.PlanSetup.PhotonCalculationModel;
            _mlctype = Check_mlc_type(ctx.PlanSetup);

            string[] calculoptions = new string[ctx.PlanSetup.GetCalculationOptions(ctx.PlanSetup.PhotonCalculationModel).Values.Count];
            calculoptions = ctx.PlanSetup.GetCalculationOptions(ctx.PlanSetup.PhotonCalculationModel).Values.ToArray();
            //MessageBox.Show(string.Format("test = {0}", calculoptions[0]));
            //MessageBox.Show(string.Format("test = {0}", calculoptions[1]));
            //_calculationgridsize = calculoptions[0];
            //SELON L'ALGO ON A DES OPTIONS ET UN NOMBRE D'OPTIONS DIFFERENT. METTRE DES IF !

            //MessageBox.Show(string.Format("Date image = {0}", ctx.Image.CreationDateTime));         
        }


        private IUCT_User GetUser(string searchtype)
        {
            string tocheck;
            switch (searchtype)
            {
                case "doctor":
                    tocheck = _ctx.PlanSetup.RTPrescription.HistoryUserName;
                    break;
                case "creator":
                    tocheck = _ctx.PlanSetup.CreationUserName;
                    break;
                default:
                    tocheck = _ctx.CurrentUser.Name;                    
                    break;
            }            


            //Generate Users list
            IUCT_Users iuct_users = new IUCT_Users();

            IUCT_User user = new IUCT_User();
            user = iuct_users.UsersList.Where(name => name.UserFamilyName == "indefini").FirstOrDefault();
            foreach (IUCT_User user_tmp in iuct_users.UsersList)
            {
                
                if (tocheck.ToLower().Contains(user_tmp.UserFamilyName.ToLower()))
                {
                    user = user_tmp;
                   
                }
            }

            //MessageBox.Show(string.Format("J'ai le current user {0}", tocheck));
            //IUCT_User user = iuct_users.UsersList.Where(tocheck.Contains(name=>name.UserFamilyName)).FirstOrDefault();
            //if (user==null)
            //{
            //    user = iuct_users.UsersList.Where(name => name.UserFamilyName == "indefini").FirstOrDefault();
            //}

            return user;             
        }

        private string Check_mlc_type(PlanSetup plan)
        {
            string technique = "Technique non reconnue (ni RA, ni DCA)";

            if (plan.Beams.Any(b => (b.MLCPlanType == VMS.TPS.Common.Model.Types.MLCPlanType.ArcDynamic)))
            {
                technique = "Arctherapie dynamique (DCA)";
            }
            if (plan.Beams.Any(b => (b.MLCPlanType == VMS.TPS.Common.Model.Types.MLCPlanType.VMAT)))
            {
                technique = "Modulation d'intensite";
            }
            if (plan.Beams.Any(b => (b.MLCPlanType == VMS.TPS.Common.Model.Types.MLCPlanType.Static)))
            {
                technique = "RTC";
            }

            return technique;
        }

        #region GETS/SETS
        public string PatientName
        {
            get { return _patientname; }
        }

        public string PatientDOB
        {
            get { return _patientdob; }
        }
        public DateTime PatientDOB_dt
        {
            get { return _patientdob_dt; }
        }
        public string CourseName
        {
            get { return _coursename; }
        }

        public string PlanName
        {
            get { return _planname; }
        }

        public IUCT_User PlanCreator
        {
            get { return _plancreator; }
        }
        public IUCT_User Doctor
        {
            get { return _doctor; }
        }

        public IUCT_User CurrentUser
        {
            get { return _currentuser; }
        }

        public string AlgoName
        {
            get { return _algoname; }
        }
        public string Mlctype
        {
            get { return _mlctype; }
        }
        //public string CalculationGridSize
        //{
        //    get { return _calculationgridsize; }
        //}
        #endregion

    }
}
