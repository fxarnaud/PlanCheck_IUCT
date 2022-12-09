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


            // an intelligent default protocol must be chosen. Still to code
            // myFullFilename = getIntelligentDefaultValue(_pcontext);

            myFullFilename = Directory.GetCurrentDirectory() + @"\protocol_check\prostate.xlsx";
            theProtocol = "Check-protocol: prostate"; // theProtocol is not a file name. It s a string that display the file name with no extension
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
                //prescriptionComment = _pcontext.PlanSetup.RTPrescription.Name;
                // prescriptionComment += " (R" + _pcontext.PlanSetup.RTPrescription.RevisionNumber + "): ";

                int nFractions = 0;
                List<double> nDosePerFraction = new List<double>();
                foreach (var target in _pcontext.PlanSetup.RTPrescription.Targets) //boucle sur les différents niveaux de dose de la prescription
                {
                    nFractions = target.NumberOfFractions;
                    nDosePerFraction.Add(target.DosePerFraction.Dose);
                }
                string listOfDoses = nFractions.ToString() + " x " + nDosePerFraction[0];
                for (int i= 1; i < nDosePerFraction.Count(); i++)
                    if (nDosePerFraction[i] != nDosePerFraction[i-1])
                        listOfDoses += "/" + nDosePerFraction[i];

                listOfDoses += " Gy (";
                prescriptionComment = listOfDoses;

                if (_pcontext.PlanSetup.RTPrescription.Notes.Length == 0)
                    prescriptionComment += "Pas de commentaire dans la presciption)";
                else
                {
                    string noEndline = _pcontext.PlanSetup.RTPrescription.Notes.Replace("\n", "").Replace("\r"," - "); // replace newline by -
                    prescriptionComment += noEndline  + ")";

                    // Just in case but revision name and number are not useful
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

        public void cleanList()
        {
            ListChecks.Clear();
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

            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.DefaultExt = "xlsx";
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory() + @"\protocol_check\";

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

            theProtocol = "Check-protocol: " + Path.GetFileNameWithoutExtension(myFullFilename);// a method to get the file name only (no extension)
            defaultProtocol.Text = theProtocol; // refresh display of default value
        }
        private void OK_button_click(object sender, RoutedEventArgs e)
        {
            this.cleanList();
            OK_button.IsEnabled = false;// Visibility.Collapsed;
            read_check_protocol rcp = new read_check_protocol(myFullFilename);


            #region THE CHECKS

            Check_Course c_course = new Check_Course(_pinfo, _pcontext);
            var check_point_course = new CheckScreen_Global(c_course.Title, c_course.Result);                                                                                               
            this.AddCheck(check_point_course);


            if (_pcontext.PlanSetup.RTPrescription != null) // faire ce check seulement si il y a une prescription
            {
                Check_Prescription c_prescri = new Check_Prescription(_pinfo, _pcontext, rcp);
                var check_point_prescription = new CheckScreen_Global(c_prescri.Title, c_prescri.Result);
                this.AddCheck(check_point_prescription);
            }


            Check_CT c_CT = new Check_CT(_pinfo, _pcontext, rcp);
            var check_point_ct = new CheckScreen_Global(c_CT.Title, c_CT.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_ct);

            /*   A FAIRE 
            Check_contours c_Contours = new Check_contours(_pinfo, _pcontext, rcp);
            var check_point_contours = new CheckScreen_Global(c_Contours.Title, c_Contours.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_contours);
            */

            Check_Isocenter c_Isocenter = new Check_Isocenter(_pinfo, _pcontext);
            var check_point_iso = new CheckScreen_Global(c_Isocenter.Title, c_Isocenter.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_iso);

            Check_Plan c_Plan = new Check_Plan(_pinfo, _pcontext, rcp);
            var check_point_plan = new CheckScreen_Global(c_Plan.Title, c_Plan.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_plan);


            Check_Model c_algo = new Check_Model(_pinfo, _pcontext, rcp);
            var check_point_model = new CheckScreen_Global(c_algo.Title, c_algo.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_model);

            /*   A FAIRE 
            Check_beams c_Beams = new Check_beams(_pinfo, _pcontext, rcp);
            var check_point_beams = new CheckScreen_Global(c_Beams.Title, c_Beams.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_beams);
            */

            Check_UM c_UM = new Check_UM(_pinfo, _pcontext);
            var check_point_um = new CheckScreen_Global(c_UM.Title, c_UM.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_um);

            /*   A FAIRE 
            Check_doseDistribution c_doseDistribution = new Check_doseDistribution(_pinfo, _pcontext);
            var check_point_dose_distribution = new CheckScreen_Global(c_doseDistribution.Title, c_doseDistribution.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_dose_distribution);
            */
            /*   A FAIRE 
            Check_finalisation c_Finalisation = new Check_finalisation(_pinfo, _pcontext);
            var check_point_finalisation = new CheckScreen_Global(c_Finalisation.Title, c_Finalisation.Result); // faire le Add check item direct pour mettre les bonnes couleurs de suite
            this.AddCheck(check_point_finalisation);
            */
            #endregion


            CheckList.Visibility = Visibility.Visible;

        }
    }
}
