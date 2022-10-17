using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Navigation;
using VMS.TPS;
using VMS.TPS.Common.Model.API;
using System.Drawing;



namespace PlanCheck_IUCT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Declarations

        private PlanSetup _plan;

        private PreliminaryInformation _pinfo;

        private ScriptContext _pcontext;
        public string PatientFullName { get; set; }
        public string PatientDOB { get; set; }
        public string CourseID { get; set; }

        public string PlanID { get; set; }

        public string PlanCreatorName { get; set; }

        public SolidColorBrush PlanCreatorBackgroundColor { get; set; }

        public SolidColorBrush PlanCreatorForegroundColor { get; set; }


        public SolidColorBrush sexBackgroundColor { get; set; }

        public SolidColorBrush sexForegroundColor { get; set; }



        public string CurrentUserName { get; set; }

        public SolidColorBrush CurrentUserBackgroundColor { get; set; }

        public SolidColorBrush CurrentUserForegroundColor { get; set; }

        public string User { get; set; }

        public Color UserColor { get; set; }


        public string PhotonModel { get; set; }
        public IEnumerable<string> CalculationOptions { get; set; }

        public string OptimizationModel { get; set; }

        public List<UserControl> ListChecks { get; set; }

        #endregion



        public MainWindow(PlanSetup plan, PreliminaryInformation pinfo,ScriptContext pcontext) //Constructeur
        {
            DataContext = this;
            _pinfo = pinfo;
            _plan = plan;
            _pcontext = pcontext;
            //Filling datas binded to xaml
            FillPreliminarytInfos();

            InitializeComponent();
        }

        public void FillPreliminarytInfos()
        {
            //Patient, plan and others infos to bind to xml
            DateTime PatientDOB = (DateTime)_pcontext.Patient.DateOfBirth;
            DateTime zeroTime = new DateTime(1, 1, 1);
            DateTime myToday = DateTime.Today;
            TimeSpan span = myToday - PatientDOB;
            int years = (zeroTime + span).Year - 1;

            String sex;
            if (_pcontext.Patient.Sex == "Female")
            {
                sex = "F";
                sexBackgroundColor = System.Windows.Media.Brushes.DeepPink;
                sexForegroundColor = System.Windows.Media.Brushes.White;
            }
            else
            {
                sex = "H";
                sexBackgroundColor = System.Windows.Media.Brushes.Blue;
                sexForegroundColor = System.Windows.Media.Brushes.White;
            }
            PatientFullName = _pinfo.PatientName + " " + sex + "/" + years.ToString();
            
             

            CourseID = _pinfo.CourseName;
            PlanID = _pinfo.PlanName;
            PlanCreatorName = _pinfo.PlanCreator.UserFamilyName;
            PlanCreatorBackgroundColor = _pinfo.PlanCreator.UserBackgroundColor;
            PlanCreatorForegroundColor = _pinfo.PlanCreator.UserForeGroundColor;
            CurrentUserName = _pinfo.CurrentUser.UserFamilyName;
            CurrentUserBackgroundColor = _pinfo.CurrentUser.UserBackgroundColor;
            CurrentUserForegroundColor = _pinfo.CurrentUser.UserForeGroundColor;

            //Plans infos
            CalculationOptions = _plan.PhotonCalculationOptions.Select(e => e.Key + " : " + e.Value);
            PhotonModel = _plan.PhotonCalculationModel;
            OptimizationModel = _plan.GetCalculationModel(CalculationType.PhotonVMATOptimization);
            OptimizationModel = _plan.GetCalculationModel(CalculationType.PhotonVMATOptimization);
            ListChecks = new List<UserControl>();


        }
        public void AddCheck(UserControl checkScreen)
        {
            ListChecks.Add(checkScreen);
            CheckList.ItemsSource = new List<UserControl>();
            CheckList.ItemsSource = ListChecks;
        }

    }
}
