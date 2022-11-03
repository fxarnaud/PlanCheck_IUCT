using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using VMS.TPS.Common.Model.API;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows;
using VMS.TPS.Common.Model.Types;
using System.Windows.Navigation;
using System.Drawing;





namespace PlanCheck_IUCT
{
    internal class Check_CT
    {
        public Check_CT(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {
            // _testpartlabel = "Algorithme";
            _context = ctx;
            _pinfo = pinfo;
            Check();
        }

        private List<Item_Result> _result = new List<Item_Result>();
        private PreliminaryInformation _pinfo;
        private ScriptContext _context;
        private string _title = "CT";

        public void Check()
        {
            Comparator testing = new Comparator();


            #region jours depuis le CT
            Item_Result CT_age = new Item_Result();
            CT_age.Label = "Ancienneté du CT (jours)";
            CT_age.ExpectedValue = "10";
            DateTime myToday = DateTime.Today;
            int nDays = (myToday - (DateTime)_context.Image.Series.HistoryDateTime).Days;
            CT_age.MeasuredValue = nDays.ToString();
            CT_age.Comparator = "<";
            CT_age.Infobulle = "Le CT doit avoir moins de 10 jours";
            CT_age.ResultStatus = testing.CompareDatas(CT_age.ExpectedValue, CT_age.MeasuredValue, CT_age.Comparator);
            this._result.Add(CT_age);
            #endregion

            #region Epaisseur de coupes
            Item_Result sliceThickness = new Item_Result();
            sliceThickness.Label = "Epaisseur de coupes (mm)";
            sliceThickness.ExpectedValue = "2.5";//XXXXX TO GET         
            sliceThickness.MeasuredValue =  _context.Image.ZRes.ToString();          
            //sliceThickness.Comparator = "=";
            sliceThickness.Infobulle = "L'épaisseur de coupe doit être "+ sliceThickness.ExpectedValue+" comme spécfifié dans le fichier Protocole";

            if(sliceThickness.ExpectedValue == sliceThickness.MeasuredValue)
                sliceThickness.setToTRUE();
            else
                sliceThickness.setToWARNING();

            //sliceThickness.ResultStatus = testing.CompareDatas(sliceThickness.ExpectedValue, sliceThickness.MeasuredValue, sliceThickness.Comparator);
            this._result.Add(sliceThickness);

            #endregion

            #region courbe HU
            Item_Result HUcurve = new Item_Result();
            String courbeHU = _context.Image.Series.ImagingDeviceId;
            String expectedHUcurve;
            
            if ((myToday - (DateTime)_context.Patient.DateOfBirth).Days < (14 * 365))
                expectedHUcurve = "Scan_IUC_100kV";
            else
                expectedHUcurve = "TDMRT";
            
            HUcurve.Label = "Courbe HU";
            HUcurve.ExpectedValue = expectedHUcurve;
            HUcurve.MeasuredValue = courbeHU;
            HUcurve.Comparator = "=";
            HUcurve.Infobulle = "La courbe doit être TDMRT sauf si âge patient < 14";
            HUcurve.ResultStatus = testing.CompareDatas(HUcurve.ExpectedValue, HUcurve.MeasuredValue, HUcurve.Comparator);
            this._result.Add(HUcurve);
            #endregion

            #region CT series number

            Item_Result deviceName = new Item_Result();
            String CT = _context.Image.Series.ImagingDeviceManufacturer + " ";
            CT = CT + _context.Image.Series.ImagingDeviceModel;
            CT = CT + _context.Image.Series.ImagingDeviceSerialNo;
            deviceName.Label = "CT series number";
            deviceName.ExpectedValue = "GE MEDICAL SYSTEMS Optima CT580";//XXXXX TO GET         
            deviceName.MeasuredValue = CT;
            deviceName.Comparator = "=";
            deviceName.Infobulle = "Vérification du modèle et du numéro de série du CT";
            deviceName.ResultStatus = testing.CompareDatas(deviceName.ExpectedValue, deviceName.MeasuredValue, deviceName.Comparator);
            this._result.Add(deviceName);
            #endregion

            #region date dans le nom imaged 3d
            
            Item_Result image3Dnaming = new Item_Result();

            image3Dnaming.Label = "Nom de l'image 3D";

            // get the CT date in format: ddmmyy
            String imageDate = ((DateTime)_context.Image.CreationDateTime).ToString("dd");
            imageDate += ((DateTime)_context.Image.CreationDateTime).ToString("MM");
            imageDate += ((DateTime)_context.Image.CreationDateTime).ToString("yy");       

            if (_context.Image.Id.Contains(imageDate))
            {
                image3Dnaming.setToTRUE();                
            }
            else
            {
                image3Dnaming.setToWARNING();                
            }

            image3Dnaming.ExpectedValue = imageDate;
            image3Dnaming.MeasuredValue = _context.Image.Id;
             image3Dnaming.Infobulle = "Le nom de l'image 3D doit contenir la date du CT au format jjmmaa ("+imageDate+")";
            //image3Dnaming.ResultStatus = testing.CompareDatas(image3Dnaming.ExpectedValue, image3Dnaming.MeasuredValue, image3Dnaming.Comparator);
            //image3Dnaming.Comparator = "=";
            this._result.Add(image3Dnaming);

            #endregion






        }

        public string Title
        {
            get { return _title; }
        }
        public List<Item_Result> Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}
