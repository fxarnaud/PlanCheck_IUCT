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



namespace PlanCheck_IUCT
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



            String msg = null;

            foreach (OptimizationParameter op in _ctx.PlanSetup.OptimizationSetup.Parameters)
            {
                OptimizationNormalTissueParameter ontp = op as OptimizationNormalTissueParameter;
                OptimizationExcludeStructureParameter oesp = op as OptimizationExcludeStructureParameter;
                OptimizationIMRTBeamParameter oibp = op as OptimizationIMRTBeamParameter;
                OptimizationJawTrackingUsedParameter ojtup = op as OptimizationJawTrackingUsedParameter;
                OptimizationPointCloudParameter opcp = op as OptimizationPointCloudParameter;


                msg += "dist to string " + ontp.DistanceFromTargetBorderInMM.ToString();
                msg += "\nend dose " + ontp.EndDosePercentage.ToString();
                msg += "\nstart dose " + ontp.StartDosePercentage.ToString();
                msg += "\nfallof " + ontp.FallOff.ToString();
                msg += "\nis auto " + ontp.IsAutomatic.ToString();
                msg += "\npriotiy " + ontp.Priority.ToString();

 
            }
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
