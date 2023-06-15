using PlanCheck.Users;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;
using System.IO;
using PlanCheck;
using System.Windows.Input;
using VMS.OIS.ARIALocal.WebServices.Document.Contracts;
using Newtonsoft.Json;
using System.Configuration;
using System.Drawing;
using System.Net.Http;
using VMS.OIS.ARIAExternal.WebServices.Documents.Contracts;



namespace PlanCheck
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
        private string _treatmentType;
        private int _treatmentFieldNumber;
        private int _setupFieldNumber;
        //  private string[] _calculoptions;
        private string[] _POoptions;
        private bool _TOMO;
        private bool _NOVA;
        private bool _HALCYON;
        private bool _HYPERARC;
        private bool _isModulated;
        private string _machine;
        private TomotherapyPdfReportReader _tprd;

        private List<DateTime> dosimetrie = new List<DateTime>();
        private List<DateTime> dosecheck = new List<DateTime>();
        private List<DateTime> ficheDePosition = new List<DateTime>();
        private List<DateTime> autres = new List<DateTime>();

        private string documentList;
        private string tomoReportPath;
        private bool tomoReportFound;

        public bool isATomoReport(String s)
        {
            bool itIs = false;
            if (s.Contains("Accuray"))
            {
                itIs = true;
                MessageBox.Show("It is a tomo report");
            }


            // Ce test doit forcément convertir en pdf pour voir si c'est une dosi TOMO 



            //String sp = Directory.GetCurrentDirectory() + @"\out\__TOTO__" + "i" + "__.txt";
            //StreamWriter sw = new StreamWriter(sp);
            //sw.WriteLine(s);
            //sw.Close();

            itIs = true;

            return itIs;
        }
        public bool isARecentDocument(DateTime t)
        {
            int recent = 20;   // number of days 
            bool returnBool = false;
            DateTime myToday = DateTime.Today;
            int nDays = (myToday - t).Days;
            if (nDays > recent)
                returnBool = false;
            else
                returnBool = true;
            return returnBool;

        }
        public bool connectToAriaDocuments(ScriptContext ctx)
        {
            bool DocumentAriaIsConnected = true;
            DocSettings docSet = DocSettings.ReadSettings();
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            string apiKeyDoc = docSet.DocKey;
            string hostName = docSet.HostName;
            string port = docSet.Port;
            tomoReportFound = false;

           // string doc1 = "Dosimétrie";
           // string doc2 = "Dosecheck";
           // string doc3 = "Fiche de positionnement";

            string response = "";
            string request = "{\"__type\":\"GetDocumentsRequest:http://services.varian.com/Patient/Documents\",\"Attributes\":[],\"PatientId\":{ \"ID1\":\"" + ctx.Patient.Id + "\"}}";
            try
            {
                response = CustomInsertDocumentsParameter.SendData(request, true, apiKeyDoc, docSet.HostName, docSet.Port);

            }
            catch
            {
                MessageBox.Show("La connexion à Aria Documents a échoué.\n La connexion ne fonctionne pas sous Citrix");
                DocumentAriaIsConnected = false;
            }
            
            if (DocumentAriaIsConnected)
                getTheAriaDocuments(response, ctx);
            
            return DocumentAriaIsConnected;
        }
        public void getTheAriaDocuments(String response, ScriptContext ctx)
        {

            #region get the list of docs with date
            DocSettings docSet = DocSettings.ReadSettings();
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            string apiKeyDoc = docSet.DocKey;
            string hostName = docSet.HostName;
            string port = docSet.Port;
            tomoReportFound = false;

            string doc1 = "Dosimétrie";
            string doc2 = "Dosecheck";
            string doc3 = "Fiche de positionnement";
            var VisitNoteList = new List<string>();
            int visitnoteloc = response.IndexOf("PtVisitNoteId");
            while (visitnoteloc > 0)
            {
                VisitNoteList.Add(response.Substring(visitnoteloc + 15, 2).Replace(",", ""));
                visitnoteloc = response.IndexOf("PtVisitNoteId", visitnoteloc + 1);
            }
            var response_Doc = JsonConvert.DeserializeObject<DocumentsResponse>(response); // get the list of documents

            var DocTypeList = new List<string>();
            var DateServiceList = new List<DateTime>();
            var PatNameList = new List<string>();
            int loopnum = 0;
            System.DateTime mostRecentDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            int mostRecentTomoReportIndex = -1;
            foreach (var document in response_Doc.Documents) // parse documents
            {


                string thePtId = document.PtId;
                string thePtVisitId = document.PtVisitId.ToString();
                string theVisitNoteId = VisitNoteList[loopnum];
                string request_docdetails = "{\"__type\":\"GetDocumentRequest:http://services.varian.com/Patient/Documents\",\"Attributes\":[],\"PatientId\":{ \"PtId\":\"" + thePtId + "\"},\"PatientVisitId\":" + thePtVisitId + ",\"VisitNoteId\":" + theVisitNoteId + "}";
                string response_docdetails = CustomInsertDocumentsParameter.SendData(request_docdetails, true, apiKeyDoc, docSet.HostName, docSet.Port);
                int typeloc = response_docdetails.IndexOf("DocumentType");
                int enteredloc = response_docdetails.IndexOf("EnteredBy");
                String thisDocType = "";
                if (typeloc > 0)
                {
                    thisDocType = response_docdetails.Substring(typeloc + 15, enteredloc - typeloc - 18);
                    DocTypeList.Add(thisDocType);
                    #region old 
                    /*
                     string saveFilePath = "";

                       if (thisDocType == doc1) // is a "Dosimétrie"
                       {
                           saveFilePath = Directory.GetCurrentDirectory() + @"\out\__" + loopnum + "__.pdf";
                           int startBinary = response_docdetails.IndexOf("\"BinaryContent\"") + 17;
                           int endBinary = response_docdetails.IndexOf("\"Certifier\"") - 2;
                           string binaryContent2 = response_docdetails.Substring(startBinary, endBinary - startBinary);
                           binaryContent2 = binaryContent2.Replace("\\", "");  // the \  makes the string a non valid base64 string
                           File.WriteAllBytes(saveFilePath, Convert.FromBase64String(binaryContent2));
                           tomoReportPath = saveFilePath;
                       }
                    */
                    #endregion
                }
                int nameloc = response_docdetails.IndexOf("PatientLastName");
                int dobloc = response_docdetails.IndexOf("PreviewText");
                if (nameloc > 0)
                {
                    PatNameList.Add(response_docdetails.Substring(nameloc + 18, dobloc - nameloc - 21));
                }

                int dateservloc = response_docdetails.IndexOf("DateOfService");
                int datesignloc = response_docdetails.IndexOf("DateSigned");
                if (dateservloc > 0)
                {
                    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(response_docdetails.Substring(dateservloc + 23, datesignloc - dateservloc - 34)) / 1000).ToLocalTime();
                    DateServiceList.Add(dtDateTime);


                    if ((thisDocType == doc1) && (isATomoReport(response_docdetails))) // looking for the last tomo report
                        if (dtDateTime > mostRecentDate)
                        {
                            mostRecentDate = dtDateTime;
                            mostRecentTomoReportIndex = loopnum;
                            MessageBox.Show("This document " + thisDocType + " " + mostRecentDate.ToString() + " " + loopnum);
                        }
                }

                loopnum++;
            }

            #region count each document type
            for (int i = 0; i < DocTypeList.Count; i++)
            {
                if (!isARecentDocument(DateServiceList[i]))
                    autres.Add(DateServiceList[i]);
                else if (DocTypeList[i] == doc1) { dosimetrie.Add(DateServiceList[i]); }
                else if (DocTypeList[i] == doc2) { dosecheck.Add(DateServiceList[i]); }
                else if (DocTypeList[i] == doc3) { ficheDePosition.Add(DateServiceList[i]); }
                else { autres.Add(DateServiceList[i]); }
            }

            documentList += "Patient: " + PatNameList[0] + " - " + ctx.Patient.Id + "\n";
            documentList += "(" + dosimetrie.Count + ") " + doc1 + ":            " + dosimetrie.DefaultIfEmpty().Max().ToString("MM/dd/yy").Replace("01/01/01", "") + "\n";
            documentList += "(" + dosecheck.Count + ") " + doc2 + ":  " + dosecheck.DefaultIfEmpty().Max().ToString("MM/dd/yy").Replace("01/01/01", "") + "\n";
            documentList += "(" + ficheDePosition.Count + ") " + doc3 + ":       " + ficheDePosition.DefaultIfEmpty().Max().ToString("MM/dd/yy").Replace("01/01/01", "") + "\n";
            documentList += "(" + autres.Count + ") " + doc3 + ":       " + autres.DefaultIfEmpty().Max().ToString("MM/dd/yy").Replace("01/01/01", "") + "\n";
            //MessageBox.Show(documentList);
            #endregion
            #endregion

            #region OLD : get the last tomo report index

            /*
             int lastTomoReportIndex = -1;
            if (dosimetrie.Count == 0)
            {
                MessageBox.Show("Pas de rapport de dosimétrie dans ARIA Documents");
                tomoReportFound = false;
            }
            else
            {
                int docIndex = -1;

                for (int i = 0; i < response_Doc.Documents.Count(); i++)
                {
                    var document = response_Doc.Documents[i];
                    string thePtId = document.PtId;
                    string thePtVisitId = document.PtVisitId.ToString();
                    string theVisitNoteId = VisitNoteList[loopnum];
                    string request_docdetails = "{\"__type\":\"GetDocumentRequest:http://services.varian.com/Patient/Documents\",\"Attributes\":[],\"PatientId\":{ \"PtId\":\"" + thePtId + "\"},\"PatientVisitId\":" + thePtVisitId + ",\"VisitNoteId\":" + theVisitNoteId + "}";
                    string response_docdetails = CustomInsertDocumentsParameter.SendData(request_docdetails, true, apiKeyDoc, docSet.HostName, docSet.Port);
                    int typeloc = response_docdetails.IndexOf("DocumentType");
                    int enteredloc = response_docdetails.IndexOf("EnteredBy");
                    if (typeloc > 0)
                    {
                        String s = response_docdetails.Substring(typeloc + 15, enteredloc - typeloc - 18);




                        if ((s == doc1))// && (isATomoReport(response_docdetails))) // it is a "dosimetrie"
                        {
                            String sp = Directory.GetCurrentDirectory() + @"\out\__" + i + "__.txt";
                            StreamWriter sw = new StreamWriter("srv0Test.txt");
                            sw.WriteLine(s);
                            sw.Close();

                        }
                    }

                }
            }
            */
            #endregion

            #region get pdf report tomo and copy in out dir
            if (_TOMO)
            {
                //foreach (var document in response_Doc.Documents) // parse documents
                //{
                var document = response_Doc.Documents[mostRecentTomoReportIndex];

                string thePtId = document.PtId;
                string thePtVisitId = document.PtVisitId.ToString();
                string theVisitNoteId = VisitNoteList[mostRecentTomoReportIndex];


                string request_docdetails = "{\"__type\":\"GetDocumentRequest:http://services.varian.com/Patient/Documents\",\"Attributes\":[],\"PatientId\":{ \"PtId\":\"" + thePtId + "\"},\"PatientVisitId\":" + thePtVisitId + ",\"VisitNoteId\":" + theVisitNoteId + "}";
                string response_docdetails = CustomInsertDocumentsParameter.SendData(request_docdetails, true, apiKeyDoc, docSet.HostName, docSet.Port);
                int typeloc = response_docdetails.IndexOf("DocumentType");
                int enteredloc = response_docdetails.IndexOf("EnteredBy");

                if (typeloc > 0)
                {
                    String s = response_docdetails.Substring(typeloc + 15, enteredloc - typeloc - 18);
                    DocTypeList.Add(s);
                    string saveFilePath = "";

                    if (s == doc1) // is a "Dosimétrie"
                    {                       
                        saveFilePath = Directory.GetCurrentDirectory() + @"\..\out\__" + loopnum + "__.pdf";
                        int startBinary = response_docdetails.IndexOf("\"BinaryContent\"") + 17;
                        int endBinary = response_docdetails.IndexOf("\"Certifier\"") - 2;
                        string binaryContent2 = response_docdetails.Substring(startBinary, endBinary - startBinary);
                        binaryContent2 = binaryContent2.Replace("\\", "");  // the \  makes the string a non valid base64 string                       
                        File.WriteAllBytes(saveFilePath, Convert.FromBase64String(binaryContent2));
                        tomoReportPath = saveFilePath;                        
                    }
                }
                //}
            }
            #endregion



        }


        // -------------------------------------------------------------------------------------------------------------------------------

        public PreliminaryInformation(ScriptContext ctx)  //Constructor
        {
            

            #region general info
            _ctx = ctx;

            if (_ctx.Patient.Name != null)
                _patientname = _ctx.Patient.Name;
            else
                _patientname = "no name";

            if (_ctx.Patient.DateOfBirth.HasValue)
            {
                _patientdob_dt = (DateTime)_ctx.Patient.DateOfBirth;
                _patientdob = _patientdob_dt.Day + "/" + _patientdob_dt.Month + "/" + _patientdob_dt.Year;
            }
            else
                _patientdob = "no DoB";


            IUCT_Users iuct_users = new IUCT_Users();

            _coursename = ctx.Course.Id;
            _planname = ctx.PlanSetup.Id;
            _plancreator = GetUser("creator", iuct_users);
            _currentuser = GetUser("currentuser", iuct_users);

            if (ctx.PlanSetup.RTPrescription != null)
                _doctor = GetUser("doctor", iuct_users);

            if (_ctx.PlanSetup.PhotonCalculationModel != null)
                _algoname = ctx.PlanSetup.PhotonCalculationModel;
            else
                _algoname = "no photon calculation model";

            _mlctype = Check_mlc_type(ctx.PlanSetup);
            /*
            _calculoptions = new string[ctx.PlanSetup.GetCalculationOptions(ctx.PlanSetup.PhotonCalculationModel).Values.Count];
            _calculoptions = ctx.PlanSetup.GetCalculationOptions(ctx.PlanSetup.PhotonCalculationModel).Values.ToArray();
            */

            int n = ctx.PlanSetup.GetCalculationOptions("PO_15605New").Values.Count;
            _POoptions = new string[n];
            _POoptions = ctx.PlanSetup.GetCalculationOptions("PO_15605New").Values.ToArray();
            #endregion
            

            #region machine
            _machine = ctx.PlanSetup.Beams.First().TreatmentUnit.Id.ToUpper();
            _NOVA = false;
            _TOMO = false;
            _HALCYON = false;
            _HYPERARC = false;
            if (_machine.Contains("NOVA"))
            {
                _NOVA = true;
                String fieldname = ctx.PlanSetup.Beams.FirstOrDefault(x => x.IsSetupField == false).Id;
                if (fieldname.Contains("HA"))
                    _HYPERARC = true;

            }
            else if (_machine.Contains("HALCYON"))
                _HALCYON = true;
            else if (_machine.Contains("TOM"))
            {
                _TOMO = true;
                foreach (PlanSetup p in ctx.Course.PlanSetups)
                {
                    if (p.Id.Contains("SEA"))
                    {
                        _machine = p.Beams.First().TreatmentUnit.Id;
                        _machine = _machine.Replace(" SEANCE", "");
                    }
                }
            }

            #endregion
           

            #region ARIA documents
            connectToAriaDocuments(ctx);
            #endregion
           

            #region set initial values

            if (_TOMO)
            {
                string pdfpath = Directory.GetCurrentDirectory() + @"\..\pdfReader\test.pdf";// @"\users\Users-IUCT.xlsx";
                _tprd = new TomotherapyPdfReportReader(pdfpath);
                _tprd.displayInfo();
            }
            else
                _tprd = null;

            foreach (Beam bn in ctx.PlanSetup.Beams)
            {

                if (bn.IsSetupField)  // count set up
                {
                    _setupFieldNumber++;
                }
                else
                {
                    _treatmentFieldNumber++;
                    //machineName = b.TreatmentUnit.Id;
                }
            }

            _isModulated = false;
            Beam b = ctx.PlanSetup.Beams.First(x => x.IsSetupField == false);

            if (b.MLCPlanType.ToString() == "VMAT")
            {
                _treatmentType = "VMAT";
                _isModulated = true;
            }
            else if (b.MLCPlanType.ToString() == "ArcDynamic")
                _treatmentType = "DCA";
            else if (b.MLCPlanType.ToString() == "DoseDynamic")
            {
                _treatmentType = "IMRT";
                _isModulated = true;
            }
            else if (b.MLCPlanType.ToString() == "Static")
                _treatmentType = "RTC (MLC)";
            else if (b.MLCPlanType.ToString() == "NotDefined")
            {
                if (b.Technique.Id == "STATIC")  // can be TOMO, Electrons or 3DCRT without MLC
                {
                    if (_machine.Contains("TOM"))
                    {
                        _treatmentType = "Tomotherapy";
                        _isModulated = true;
                    }
                    else if (b.EnergyModeDisplayName.Contains("E"))
                        _treatmentType = "Electrons";
                    else
                        _treatmentType = "RTC (sans MLC)";
                }
                else
                    _treatmentType = "Technique non statique inconnue : pas de MLC !";
            }
            #endregion
            
        }


        private IUCT_User GetUser(string searchtype, IUCT_Users iuct_users)
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
            //IUCT_Users iuct_users = new IUCT_Users();

            IUCT_User user = new IUCT_User();
            user = iuct_users.UsersList.Where(name => name.UserFamilyName == "indefini").FirstOrDefault();
            foreach (IUCT_User user_tmp in iuct_users.UsersList)
            {


                if (tocheck.ToLower().Contains(user_tmp.UserFamilyName.ToLower()))
                {
                    user = user_tmp;

                }
            }



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
        /*
         public string[] Calculoptions
         {
             get { return _calculoptions; }
         }
        */
        public string[] POoptions
        {
            get { return _POoptions; }
        }
        public string PatientDOB
        {
            get { return _patientdob; }
        }
        public string documentsList
        {
            get { return documentList; }
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
        public bool isModulated
        {
            get { return _isModulated; }
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
        public string treatmentType
        {
            get { return _treatmentType; }
        }
        public int treatmentFieldNumber
        {
            get { return _treatmentFieldNumber; }
        }
        public int setupFieldNumber
        {
            get { return _setupFieldNumber; }
        }

        public void setTreatmentType(string type)
        {
            _treatmentType = type;
        }
        public bool isTOMO
        {
            get { return _TOMO; }
        }
        public bool isNOVA
        {
            get { return _NOVA; }
        }
        public bool isHALCYON
        {
            get { return _HALCYON; }
        }
        public bool isHyperArc
        {
            get { return _HYPERARC; }
        }
        public string machine
        {
            get { return _machine; }
        }
        public TomotherapyPdfReportReader tprd
        {
            get { return _tprd; }
        }

        public string tomoReportPathFile
        {
            get { return tomoReportPath; }
        }
        public bool tomoReportIsFound
        {
            get { return tomoReportFound; }
        }
        #endregion

    }
}
