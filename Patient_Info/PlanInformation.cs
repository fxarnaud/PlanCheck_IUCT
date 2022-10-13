using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck_IUCT
{
    internal class PlanInformation
    {
        string _planname;
        string _patientname;
        string _coursename;
        string _algoname;
        string _mlctype;

        public PlanInformation(ScriptContext ctx)  //Constructor
        {
            _patientname = ctx.Patient.Name;
            _coursename = ctx.Course.Id;
            _planname = ctx.PlanSetup.Id;
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

        public string PlanName
        {
            get { return _planname; }
        }
        public string CourseName
        {
            get { return _coursename; }
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
