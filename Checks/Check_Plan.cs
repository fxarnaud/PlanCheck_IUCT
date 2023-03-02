using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using System.Windows;
using System.Windows.Navigation;
using System.Drawing;



namespace PlanCheck
{
    internal class Check_Plan
    {

        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;
        private read_check_protocol _rcp;
        public Check_Plan(PreliminaryInformation pinfo, ScriptContext ctx, read_check_protocol rcp)  //Constructor
        {
            _rcp = rcp;
            _ctx = ctx;
            _pinfo = pinfo;
            Check();
        }

        public bool CompareNTO(OptimizationNormalTissueParameter planNTO, NTO protocolNTO)
        {
            bool result = true;

            if (planNTO.DistanceFromTargetBorderInMM != protocolNTO.distanceTotTarget) result = false;
            if (planNTO.StartDosePercentage != protocolNTO.startPercentageDose) result = false;
            if (planNTO.EndDosePercentage != protocolNTO.stopPercentageDose) result = false;
            if (planNTO.FallOff != protocolNTO.falloff) result = false;
            if (planNTO.Priority != protocolNTO.priority) result = false;

            bool autoMode = false;
            if (protocolNTO.mode == "Manual") autoMode = false;
            else if (protocolNTO.mode == "Auto") autoMode = true;

            if (planNTO.IsAutomatic != autoMode) result = false;

            return result;



        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Plan";

        public void Check()
        {

            #region Gating
            Item_Result gating = new Item_Result();
            gating.Label = "Gating";

            if (_ctx.PlanSetup.UseGating)
                gating.MeasuredValue = "Gating activé";
            else
                gating.MeasuredValue = "Gating Désactivé";

            if (_rcp.enebleGating == "Oui")
                gating.ExpectedValue = "Gating activé";
            if (_rcp.enebleGating == "Non")
                gating.ExpectedValue = "Gating Désactivé";

            if (gating.ExpectedValue == gating.MeasuredValue)
                gating.setToTRUE();
            else
                gating.setToFALSE();

            gating.Infobulle = "La case Enable gating doit être en accord avec le check-protocol " + _rcp.protocolName + " (" + gating.ExpectedValue + ")";
            this._result.Add(gating);
            #endregion

            #region NTO



            if (_ctx.PlanSetup.OptimizationSetup.Parameters.Count() > 0) // if there is an optim. pararam
            {
                //foreach (OptimizationObjective oo in _ctx.PlanSetup.OptimizationSetup.Objectives)
                // foreach (OptimizationParameter op in _ctx.PlanSetup.OptimizationSetup.Parameters)




                OptimizationNormalTissueParameter ontp = _ctx.PlanSetup.OptimizationSetup.Parameters.FirstOrDefault(x => x.GetType().Name == "OptimizationNormalTissueParameter") as OptimizationNormalTissueParameter;

                // OptimizationNormalTissueParameter ontp = op as OptimizationNormalTissueParameter;
                bool NTOparamsOk = CompareNTO(ontp, _rcp.NTOparams);

                Item_Result NTO = new Item_Result();
                NTO.Label = "NTO";
                if (NTOparamsOk)
                {
                    NTO.MeasuredValue = "Paramètres NTO conformes au protocole";
                    NTO.Infobulle = "Paramètres NTO conformes au protocole " + _rcp.protocolName;
                    NTO.setToTRUE();
                }
                else
                {
                    NTO.MeasuredValue = "Paramètres NTO non conformes au protocole";
                    NTO.Infobulle = "Paramètres NTO non conformes au protocole " + _rcp.protocolName;



                    NTO.setToFALSE();
                }
                NTO.Infobulle += "\n Paramètres NTO du plan :";
                NTO.Infobulle += "\n Distance : " + ontp.DistanceFromTargetBorderInMM;
                NTO.Infobulle += "\n Fall off : " + ontp.FallOff;
                NTO.Infobulle += "\n Start Dose : " + ontp.StartDosePercentage;
                NTO.Infobulle += "\n End Dose : " + ontp.EndDosePercentage;
                NTO.Infobulle += "\n Priority : " + ontp.Priority;
                NTO.Infobulle += "\n Auto Mode : " + ontp.IsAutomatic;
                if (ontp.IsAutomatic)
                    NTO.Infobulle += " (Auto)";
                else
                    NTO.Infobulle += " (Manual)";

                NTO.Infobulle += "\n Paramètres NTO du protocole :";
                NTO.Infobulle += "\n Distance : " + _rcp.NTOparams.distanceTotTarget;
                NTO.Infobulle += "\n Fall off : " + _rcp.NTOparams.falloff;
                NTO.Infobulle += "\n Start Dose : " + _rcp.NTOparams.startPercentageDose;
                NTO.Infobulle += "\n End Dose : " + _rcp.NTOparams.stopPercentageDose;
                NTO.Infobulle += "\n Priority : " + _rcp.NTOparams.priority;
                NTO.Infobulle += "\n Auto Mode : " + _rcp.NTOparams.mode;
                this._result.Add(NTO);


                /*
                OptimizationExcludeStructureParameter oesp = op as OptimizationExcludeStructureParameter;
                OptimizationIMRTBeamParameter oibp = op as OptimizationIMRTBeamParameter;
                OptimizationJawTrackingUsedParameter ojtup = op as OptimizationJawTrackingUsedParameter;
                OptimizationPointCloudParameter opcp = op as OptimizationPointCloudParameter;

                */


            }


            #endregion

            // MessageBox.Show(_ctx.PlanSetup.OptimizationSetup.Parameters.Count() + " " + msg);

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

