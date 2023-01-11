using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows;
using System.Windows.Navigation;




namespace PlanCheck_IUCT
{
    internal class Check_Prescription
    {
        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;
        private read_check_protocol _rcp;
        public Check_Prescription(PreliminaryInformation pinfo, ScriptContext ctx, read_check_protocol rcp)  //Constructor
        {
            _ctx = ctx;
            _pinfo = pinfo;
            _rcp = rcp;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Prescription";

        public void Check()
        {

            #region LISTE DES CIBLES DE LA PRESCRIPTION
            Item_Result prescriptionVolumes = new Item_Result();

            int targetNumber = 0;
            prescriptionVolumes.MeasuredValue = "";
            prescriptionVolumes.Infobulle = "information : liste des cibles de la prescription\n";
            foreach (var target in _ctx.PlanSetup.RTPrescription.Targets) //boucle sur les différents niveaux de dose de la prescription
            {
                targetNumber++;
                double tot = target.NumberOfFractions * target.DosePerFraction.Dose;
                prescriptionVolumes.Infobulle += target.TargetId + " : " + target.NumberOfFractions + " x " + target.DosePerFraction.Dose + " Gy " + "(" + tot.ToString("N2") + " Gy)\n";
                prescriptionVolumes.MeasuredValue += target.TargetId + " (" + tot.ToString("N2") + " Gy)  ";
            }

            prescriptionVolumes.ExpectedValue = "info";
            if (_ctx.PlanSetup.RTPrescription.Status == "Approved")
            {
                prescriptionVolumes.Label = " Prescription approuvée pour " + targetNumber + " cible(s) : ";
                prescriptionVolumes.setToTRUE();
            }
            else
            {
                prescriptionVolumes.Label = " Prescription non approuvée (" + targetNumber + " cible(s))";
                prescriptionVolumes.setToFALSE();
            }

            this._result.Add(prescriptionVolumes);

            #endregion

            /*
            #region APPROBATION DE LA PRESCRIPTION
            Item_Result prescriptionStatus = new Item_Result();
            prescriptionStatus.Label = "Approbation de la prescription (" + _ctx.PlanSetup.RTPrescription.Name + ")";
            prescriptionStatus.ExpectedValue = "Approved";
            prescriptionStatus.MeasuredValue = _ctx.PlanSetup.RTPrescription.Status;

            if (prescriptionStatus.MeasuredValue == "Approved")
                prescriptionStatus.setToTRUE();
            else
                prescriptionStatus.setToFALSE();


            prescriptionStatus.Infobulle = "OK si la prescription est approuvée";
            this._result.Add(prescriptionStatus);
            #endregion
            */
            #region FRACTIONNEMENT - CIBLE LA PLUS HAUTE
            Item_Result fractionation = new Item_Result();
            //fractionation.Label = "Fractionnement du PTV principal";

            double nPrescribedDosePerFraction = 0;
            int nPrescribedNFractions = 0;
            string PrescriptionName = null;
            double PrescriptionValue = 0;
            DoseValue myDosePerFraction = _ctx.PlanSetup.DosePerFraction;
            int nFraction = (int)_ctx.PlanSetup.NumberOfFractions;
            foreach (var target in _ctx.PlanSetup.RTPrescription.Targets) //boucle sur les différents niveaux de dose de la prescription
            {
                // MessageBox.Show("one : " + target.Name);
                nPrescribedNFractions = target.NumberOfFractions;
                if (target.DosePerFraction.Dose > nPrescribedDosePerFraction)  // get the highest dose per fraction level
                {
                    nPrescribedDosePerFraction = target.DosePerFraction.Dose;
                    PrescriptionValue = target.Value;
                    PrescriptionName = target.TargetId;

                    //if(PrescriptionName !=null)
                    // 
                }
            }

            fractionation.Label = "Fractionnement de la cible principale (" + PrescriptionName + ")";
            fractionation.ExpectedValue = nPrescribedNFractions + " x " + nPrescribedDosePerFraction + " Gy";
            fractionation.MeasuredValue = "Plan : " + nFraction + " x " + myDosePerFraction.Dose.ToString("0.00") + " Gy - Prescrits : " + nPrescribedNFractions + " x " + nPrescribedDosePerFraction.ToString("0.00") + " Gy";

            if ((nPrescribedNFractions == nFraction) && (nPrescribedDosePerFraction == myDosePerFraction.Dose))
                fractionation.setToTRUE();
            else
                fractionation.setToFALSE();


            fractionation.Infobulle = "Le 'nombre de fractions' et la 'dose par fraction' du plan doivent\nêtre conformes à la prescription " + _ctx.PlanSetup.RTPrescription.Id +
                " : " + nPrescribedNFractions.ToString() + " x " + nPrescribedDosePerFraction + " Gy.\n\n Le système récupère la dose la plus haute prescrite\nsi il existe plusieurs niveaux de dose dans la prescription";
            this._result.Add(fractionation);
            #endregion


            // pas réussi à attraper le % dans la prescription (que dans le plan)
            #region POURCENTAGE DE LA PRESCRIPTION

            Item_Result percentage = new Item_Result();
            double myTreatPercentage = _ctx.PlanSetup.TreatmentPercentage;
            myTreatPercentage = 100 * myTreatPercentage;
            percentage.Label = "Pourcentage de traitement";
            percentage.ExpectedValue = _rcp.prescriptionPercentage;
            percentage.MeasuredValue = myTreatPercentage.ToString() + "%";
            if (percentage.ExpectedValue == percentage.MeasuredValue)
                percentage.setToTRUE();
            else
                percentage.setToFALSE();
            percentage.Infobulle = "Le pourcentage de traitement (onglet Dose) doit être en accord avec";
            percentage.Infobulle += "\nla valeur de pourcentage du check-protocol " + _rcp.protocolName + " (" + _rcp.prescriptionPercentage + ")";
            this._result.Add(percentage);
            #endregion



            #region NORMALISATION DU PLAN
            Item_Result normalisation = new Item_Result();
            //string normMethod = _ctx.PlanSetup.PlanNormalizationMethod;
            normalisation.Label = "Mode de normalisation du plan";
            normalisation.ExpectedValue = _rcp.normalisationMode;
            normalisation.MeasuredValue = _ctx.PlanSetup.PlanNormalizationMethod;

            if (normalisation.MeasuredValue.Contains("volume")) // si le mode de normalisation contient le mot volume
            {
                if (normalisation.ExpectedValue == normalisation.MeasuredValue)
                    normalisation.setToTRUE();
                else
                    normalisation.setToFALSE();

                normalisation.MeasuredValue += ": " + _ctx.PlanSetup.TargetVolumeID; // afficher ce volume

            }
            if (normalisation.MeasuredValue.Contains("point"))
            {
                if (normalisation.MeasuredValue.Contains("100% au point de référence"))
                {
                    if (normalisation.ExpectedValue.Contains("100% au point de référence"))
                        normalisation.setToTRUE();
                    else
                        normalisation.setToFALSE();

                    if (normalisation.MeasuredValue.Contains("principal"))
                        normalisation.MeasuredValue += " (" + _ctx.PlanSetup.PrimaryReferencePoint.Id + ")";

                }
                else
                {
                    normalisation.setToFALSE();
                }
            }

            //if (normMethod == "100.00% couvre 50.00% du volume cible")
            //  normalisation.MeasuredValue = normMethod + " au " + _ctx.PlanSetup.TargetVolumeID;
            //else if (normMethod == "100% au point de référence principal")
            //  normalisation.MeasuredValue = normMethod + " au point " + _ctx.PlanSetup.PrimaryReferencePoint.Id;



            normalisation.Infobulle = "Le mode de normalisation (onglet Dose) doit être en accord avec le check-protocol. Cet item est en WARNING si Aucune normalisation";



            if (normalisation.MeasuredValue == "Aucune normalisation de plan")
                normalisation.setToWARNING();

            this._result.Add(normalisation);
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
