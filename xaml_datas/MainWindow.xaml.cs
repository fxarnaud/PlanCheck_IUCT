using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.IO;
using System.Text;



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
        public string strPatientDOB { get; set; }
        public string PlanAndCourseID { get; set; }
        public string prescriptionComment { get; set; }
        public string PlanCreatorName { get; set; }
        public SolidColorBrush PlanCreatorBackgroundColor { get; set; }
        public SolidColorBrush PlanCreatorForegroundColor { get; set; }
        public SolidColorBrush sexBackgroundColor { get; set; }
        public SolidColorBrush sexForegroundColor { get; set; }
        public string machineBackgroundColor { get; set; }
        public string machineForegroundColor { get; set; }
        public string CurrentUserName { get; set; }
        public SolidColorBrush CurrentUserBackgroundColor { get; set; }
        public SolidColorBrush CurrentUserForegroundColor { get; set; }
        public string DoctorName { get; set; }
        public SolidColorBrush DoctorBackgroundColor { get; set; }
        public SolidColorBrush DoctorForegroundColor { get; set; }
        public string User { get; set; }
        public Color UserColor { get; set; }
        public string theMachine { get; set; }
        public string theFields { get; set; }
        public string theProtocol { get; set; }        
        public string myFullFilename { get; set; }
        public string PhotonModel { get; set; }
        public IEnumerable<string> CalculationOptions { get; set; }
        public string OptimizationModel { get; set; }
        public List<UserControl> ListChecks { get; set; }

        #endregion

        

        public MainWindow(PreliminaryInformation pinfo, ScriptContext pcontext) //Constructeur
        {
            DataContext = this;
            _pinfo = pinfo;
            _plan = pcontext.PlanSetup;
            _pcontext = pcontext;

           // myFullFilename = getIntelligentDefaultValue(_pcontext);

            myFullFilename = @"\\srv015\SF_COM\ARNAUD_FX\ECLIPSE_SCRIPTING\Plan_Check_new\check_protocol\prostate.xlsx"; //default_path";
            theProtocol = "Check-protocol: prostate";

            FillHeaderInfos(); //Filling datas binded to xaml

            InitializeComponent(); // not clear what is done here
           

        }

        public void FillHeaderInfos()
        {
            //Patient, plan and others infos to bind to xml

            #region PATIENT NAME, SEX AND AGE
            DateTime PatientDOB = (DateTime)_pinfo.PatientDOB_dt;// .Patient.DateOfBirth;         
            DateTime zeroTime = new DateTime(1, 1, 1);
            DateTime myToday = DateTime.Today;
            TimeSpan span = myToday - _pinfo.PatientDOB_dt;
            int years = (zeroTime + span).Year - 1;
            String sex;
            if (_pcontext.Patient.Sex == "Female")
            {
                sex = "F";
                //sexBackgroundColor = System.Windows.Media.Brushes.DeepPink;
                sexBackgroundColor = System.Windows.Media.Brushes.Wheat;
                // sexForegroundColor = System.Windows.Media.Brushes.White;
                sexForegroundColor = System.Windows.Media.Brushes.DeepPink;
                strPatientDOB = "Née le " + _pinfo.PatientDOB; // for tooltip only
            }
            else
            {
                sex = "H";
                //sexBackgroundColor = System.Windows.Media.Brushes.Blue;
                //sexForegroundColor = System.Windows.Media.Brushes.White;
                sexBackgroundColor = System.Windows.Media.Brushes.Wheat;
                sexForegroundColor = System.Windows.Media.Brushes.Blue;
                strPatientDOB = "Né le " + _pinfo.PatientDOB; // for tooltip only
            }
            PatientFullName = _pinfo.PatientName + " " + sex + "/" + years.ToString();
            #endregion

            #region course and plan ID format:  PlanID (CourseID)

            PlanAndCourseID = _pinfo.PlanName + " (" + _pinfo.CourseName + ")";

            #endregion

            #region creator name

            PlanCreatorName = "    " + _pinfo.PlanCreator.UserFirstName + " " + _pinfo.PlanCreator.UserFamilyName;
            PlanCreatorBackgroundColor = _pinfo.PlanCreator.UserBackgroundColor;
            PlanCreatorForegroundColor = _pinfo.PlanCreator.UserForeGroundColor;
            #endregion

            #region User
            CurrentUserName = "    " + _pinfo.CurrentUser.UserFirstName + " " + _pinfo.CurrentUser.UserFamilyName;
            CurrentUserBackgroundColor = _pinfo.CurrentUser.UserBackgroundColor;
            CurrentUserForegroundColor = _pinfo.CurrentUser.UserForeGroundColor;
            #endregion

            #region doctor in the prescription
            if (_pcontext.PlanSetup.RTPrescription != null)
            {
                DoctorName = "    " + "Dr " + _pinfo.Doctor.UserFamilyName + "    ";
                DoctorBackgroundColor = _pinfo.Doctor.UserBackgroundColor; //System.Windows.Media.Brushes.DeepPink; // _pinfo.Doctor.DoctorBackgroundColor;
                DoctorForegroundColor = _pinfo.Doctor.UserForeGroundColor;// System.Windows.Media.Brushes.Wheat; // _pinfo.Doctor.DoctorForeGroundColor;
            }
            else DoctorName = "    " + "Pas de prescripteur";
            #endregion

            #region prescription comment
            if (_pcontext.PlanSetup.RTPrescription != null)
            {
                prescriptionComment = _pcontext.PlanSetup.RTPrescription.Name;
                prescriptionComment += " (R" + _pcontext.PlanSetup.RTPrescription.RevisionNumber + "): ";
                if (_pcontext.PlanSetup.RTPrescription.Notes.Length == 0)
                    prescriptionComment += "Pas de commentaire dans la presciption.";
                else
                {

                    prescriptionComment += _pcontext.PlanSetup.RTPrescription.Notes;

                    //+ _pcontext.PlanSetup.RTPrescription.RevisionNumber + ": " + ": " + _pcontext.PlanSetup.RTPrescription.Id + ": " + _pcontext.PlanSetup.RTPrescription.Notes;
                    //prescriptionComment = "Commentaire de la presciption : " + _pcontext.PlanSetup.RTPrescription.Notes;

                }
            }
            else
                prescriptionComment = "pas de prescription";
            #endregion

            #region machine and fields
            String machineName = null;
            String treatmentType = null;
            int setupFieldNumber = 0;
            int TreatmentFieldNumber = 0;
            //String monTypeMLC = null;
            foreach (Beam b in _pcontext.PlanSetup.Beams)
            {

                if (b.IsSetupField)  // count set up
                {
                    setupFieldNumber++;
                }
                else
                {
                    TreatmentFieldNumber++;
                    machineName = b.TreatmentUnit.Id;


                    if (b.MLCPlanType.ToString() == "VMAT")
                    {
                        treatmentType = "VMAT";

                    }
                    else if (b.MLCPlanType.ToString() == "ArcDynamic")
                        treatmentType = "DCA";
                    else if (b.MLCPlanType.ToString() == "DoseDynamic")
                        treatmentType = "IMRT";
                    else if (b.MLCPlanType.ToString() == "Static")
                        treatmentType = "RTC (MLC)";
                    else if (b.MLCPlanType.ToString() == "NotDefined")
                    {
                        if (b.Technique.Id == "STATIC")  // can be TOMO, Electrons or 3DCRT without MLC
                        {
                            if (machineName == "TOM")
                                treatmentType = "Tomotherapy";
                            else if (b.EnergyModeDisplayName.Contains("E"))
                                treatmentType = "Electrons";
                            else
                                treatmentType = "RTC (sans MLC)";
                        }
                        else
                            treatmentType = "Technique non statique inconnue : pas de MLC !";
                    }
                }
            }

            theMachine = "    " + machineName;

            #region color the machines first theme

            // see palette at https://learn.microsoft.com/fr-fr/dotnet/api/system.windows.media.brushes?view=windowsdesktop-6.0


            if (machineName == "V4")
            {
                machineBackgroundColor = "PowderBlue";
                machineForegroundColor = "Blue";
            }
            else if (machineName == "TOM")
            {
                machineBackgroundColor = "Orange";
                machineForegroundColor = "White";
            }
            else if (machineName == "TOMO2")
            {
                machineBackgroundColor = "Orange";
                machineForegroundColor = "White";
            }
            else if (machineName == "NOVA3")
            {
                machineBackgroundColor = "Green";
                machineForegroundColor = "White";
            }
            else if (machineName == "TOMO4")
            {
                machineBackgroundColor = "Red";
                machineForegroundColor = "White";
            }
            else if (machineName == "NOVA5")
            {
                machineBackgroundColor = "Yellow";
                machineForegroundColor = "Black";
            }
            else if (machineName == "NOVA SBRT")
            {
                machineBackgroundColor = "Gold";
                machineForegroundColor = "Blue";
            }
            else if (machineName == "HALCYON6")
            {
                machineBackgroundColor = "LightBlue";
                machineForegroundColor = "White";
            }
            else if (machineName == "TOMO7")
            {
                machineBackgroundColor = "Brown";
                machineForegroundColor = "White";
            }
            else if (machineName == "HALCYON8")
            {
                machineBackgroundColor = "DeepSkyBlue";
                machineForegroundColor = "White";
            }
            else
            {
                machineBackgroundColor = "Gray";
                machineForegroundColor = "White";
            }

            #endregion



            if (machineName != "TOM")
            {
                if (TreatmentFieldNumber == 1)
                    theFields = treatmentType + " : " + TreatmentFieldNumber + " champ + " + setupFieldNumber + " set-up";
                else
                    theFields = treatmentType + " : " + TreatmentFieldNumber + " champs + " + setupFieldNumber + " set-up";
                // theFields = TreatmentFieldNumber + " champs " + treatmentType + " et " + setupFieldNumber + " champs de set-up" ;
            }
            else
                theFields = "Tomotherapy";
            //MessageBox.Show(machineAndFields);
            #endregion

            #region other infos
            //Plans infos
            CalculationOptions = _plan.PhotonCalculationOptions.Select(e => e.Key + " : " + e.Value);
            PhotonModel = _plan.PhotonCalculationModel;
            OptimizationModel = _plan.GetCalculationModel(CalculationType.PhotonVMATOptimization);
            OptimizationModel = _plan.GetCalculationModel(CalculationType.PhotonVMATOptimization);
            ListChecks = new List<UserControl>();
            #endregion

        }
        public void AddCheck(UserControl checkScreen)
        {
            ListChecks.Add(checkScreen);
            CheckList.ItemsSource = new List<UserControl>();
            CheckList.ItemsSource = ListChecks;
        }
        private void Choose_file_button_Click(object sender, RoutedEventArgs e)
        {
            OK_button.IsEnabled = true;
            //String myFileName;
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.DefaultExt = "xlsx";
            fileDialog.InitialDirectory = @"\\srv015\SF_COM\ARNAUD_FX\ECLIPSE_SCRIPTING\Plan_Check_new\check_protocol\";
            if (!Directory.Exists(fileDialog.InitialDirectory))
            {
                MessageBox.Show(fileDialog.InitialDirectory + "n'existe pas.");
                fileDialog.InitialDirectory = @"C:\";
            }

            fileDialog.Multiselect = false;
            fileDialog.Title = "Selection du check-protocol";
            fileDialog.ShowReadOnly = true;
            fileDialog.Filter = "XLSX files (*.xlsx)|*.xlsx";
            fileDialog.FilterIndex = 0;
            fileDialog.CheckFileExists = true;
            if (fileDialog.ShowDialog() == false)
            {
                return;    // user canceled
            }
            myFullFilename = fileDialog.FileName; // full absolute path                                                  
            if (!System.IO.File.Exists(myFullFilename))
            {
                MessageBox.Show(string.Format("Le check-protocol '{0}'  n'existe pas ", theProtocol));
                return;
            }
            //myFileName = Path.GetFileName(myFullFilename); // a method to get the file name only
            theProtocol = "Check-protocol: "+Path.GetFileNameWithoutExtension(myFullFilename);// a method to get the file name only (ne extension)
            defaultProtocol.Text = theProtocol; // refresh display of default value
        }
        private void OK_button_click(object sender, RoutedEventArgs e)
        {
            OK_button.IsEnabled = false;// Visibility.Collapsed;
            read_check_protocol rcp = new read_check_protocol(myFullFilename);


            #region THE CHECKS
            Check_Course c_course = new Check_Course(_pinfo, _pcontext);
            var check_point1 = new CheckScreen_Global(c_course.Title, c_course.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
           // window.AddCheck(check_point1);
            this.AddCheck(check_point1);


            Check_CT c_CT = new Check_CT(_pinfo, _pcontext, rcp);
            var check_point2 = new CheckScreen_Global(c_CT.Title, c_CT.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point2);


            if (_pcontext.PlanSetup.RTPrescription != null)
            {
                Check_Prescription c_prescri = new Check_Prescription(_pinfo, _pcontext, rcp);
                var check_point3 = new CheckScreen_Global(c_prescri.Title, c_prescri.Result);
                this.AddCheck(check_point3);
            }

            Check_Plan c_algo = new Check_Plan(_pinfo, _pcontext, rcp);
            var check_point4 = new CheckScreen_Global(c_algo.Title, c_algo.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point4);


            Check_UM c_UM = new Check_UM(_pinfo, _pcontext);
            var check_point5 = new CheckScreen_Global(c_UM.Title, c_UM.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point5);


            Check_Isocenter c_Isocenter = new Check_Isocenter(_pinfo, _pcontext);
            var check_point6 = new CheckScreen_Global(c_Isocenter.Title, c_Isocenter.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point6);


            Check_contours c_Contours = new Check_contours(_pinfo, _pcontext);
            var check_point7 = new CheckScreen_Global(c_Contours.Title, c_Contours.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point7);

            Check_beams c_Beams = new Check_beams(_pinfo, _pcontext, rcp);
            var check_point8 = new CheckScreen_Global(c_Beams.Title, c_Beams.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point8);


            Check_doseDistribution c_doseDistribution = new Check_doseDistribution(_pinfo, _pcontext);
            var check_point9 = new CheckScreen_Global(c_doseDistribution.Title, c_doseDistribution.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point9);

            Check_finalisation c_Finalisation = new Check_finalisation(_pinfo, _pcontext);
            var check_point10 = new CheckScreen_Global(c_Finalisation.Title, c_Finalisation.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point10);
            #endregion

            
            CheckList.Visibility = Visibility.Visible;

        }
    }
}
