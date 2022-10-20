using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck_IUCT
{
    internal class Check_Prescription
    {
        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;
        
        public Check_Prescription(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {     
            _ctx = ctx;
            _pinfo = pinfo;
            Check();
            
        }

        private List<Item_Result> _result = new List<Item_Result>();
       // private PreliminaryInformation _pinfo;
        private string _title = "Prescription";

        public void Check()
        {
            #region APPROBATION DE LA PRESCRIPTION
            Item_Result prescriptionStatus = new Item_Result();
            prescriptionStatus.Label =  "Approbation de la prescription (" + _ctx.PlanSetup.RTPrescription.Name + ")";
            prescriptionStatus.ExpectedValue = "Approved";
            prescriptionStatus.MeasuredValue = _ctx.PlanSetup.RTPrescription.Status;

            if (prescriptionStatus.MeasuredValue == "Approved") 
                prescriptionStatus.setToTRUE();
            else
                prescriptionStatus.setToFALSE();


            prescriptionStatus.Infobulle = "La prescription doit être approuvée"; 
            this._result.Add(prescriptionStatus);
            #endregion

            #region FRACTIONNEMENT - PTV LE PLUS HAUT
            Item_Result fractionation = new Item_Result();
            fractionation.Label = "Fractionnement du PTV HD";

            double nPrescribedDosePerFraction = 0;
            int nPrescribedNFractions = 0;
            string PrescriptionName;
            double PrescriptionValue = 0;
            DoseValue myDosePerFraction = _ctx.PlanSetup.DosePerFraction;
            int nFraction = (int)_ctx.PlanSetup.NumberOfFractions;
            foreach (var target in _ctx.PlanSetup.RTPrescription.Targets) //boucle sur les différents niveaux de dose de la prescription
            {
                nPrescribedNFractions = target.NumberOfFractions;              
                if (target.DosePerFraction.Dose > nPrescribedDosePerFraction)  // get the highest dose per fraction level
                {
                    nPrescribedDosePerFraction = target.DosePerFraction.Dose;
                    PrescriptionValue = target.Value;
                    PrescriptionName = target.Name;
                }
            }
            fractionation.ExpectedValue = nPrescribedNFractions+" x " + nPrescribedDosePerFraction +  " Gy" ;


            fractionation.MeasuredValue = nFraction + " x " + myDosePerFraction.Dose.ToString() + " Gy";

            if ((nPrescribedNFractions == nFraction) && (nPrescribedDosePerFraction == myDosePerFraction.Dose))
                fractionation.setToTRUE();
            else
                fractionation.setToFALSE();


            fractionation.Infobulle = "Le nombre de séances et la dose par séance du plan doivent\nêtre conforme à la prescription du PTV ayant la plus forte dose prescrite : "+ nPrescribedNFractions.ToString() + " x " + nPrescribedDosePerFraction + " Gy";
            this._result.Add(fractionation);
            #endregion

            #region LISTE DES VOLUMES DE LA PRESCRIPTION
            Item_Result prescriptionVolumes = new Item_Result();
            prescriptionVolumes.Label = "Liste des volumes à traiter";
            prescriptionVolumes.ExpectedValue = "info";


            int targetNumber = 0;
            prescriptionVolumes.MeasuredValue = "";
            foreach (var target in _ctx.PlanSetup.RTPrescription.Targets) //boucle sur les différents niveaux de dose de la prescription
            {
                targetNumber++;
                // ItemResult myItem00 = new ItemResult();
                //myItem00.testID = "4.2." + targetNumber.ToString();
                //myItem00.Label = "  Target " + targetNumber.ToString() + ": " + target.TargetId;
                double tot = target.NumberOfFractions * target.DosePerFraction.Dose;
                prescriptionVolumes.MeasuredValue += target.TargetId + " : " + target.NumberOfFractions + " x " + target.DosePerFraction.Dose + " Gy " + "(" + tot.ToString("N2") + " Gy)\t";
                //mymsg = (target.DosePerFraction.Dose * target.NumberOfFractions) + "/";
                //mymsg = mymsg + target.DosePerFraction.Dose;
                //myItem00.MeasuredValue = mymsg;
               // myItem00.ExpectedValue = "Info only (no check)";
               // myItem00.Objective = "=";
               // myItem00.ResultStatus = testing.CompareDatas(myItem00.ExpectedValue, myItem00.MeasuredValue, myItem00.Objective);
               // this._result.Add(myItem00);


            }

            //prescriptionVolumes.MeasuredValue = _ctx.PlanSetup.RTPrescription.Status;

            //if (prescriptionVolumes.MeasuredValue == "Approved")


            prescriptionVolumes.setToINFO();
            //else                 prescriptionVolumes.setToFALSE();


            prescriptionVolumes.Infobulle = "information : liste des volumes de la prescription";
            this._result.Add(prescriptionVolumes);
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
