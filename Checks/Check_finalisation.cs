using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck_IUCT
{
    internal class Check_finalisation
    {
        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;
        private read_check_protocol _rcp;
        public Check_finalisation(PreliminaryInformation pinfo, ScriptContext ctx, read_check_protocol rcp)  //Constructor
        {
            _ctx = ctx;
            _pinfo = pinfo;
            _rcp = rcp;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Finalisation";
        private bool haveTheSameMU(PlanSetup p1, PlanSetup p2)
        {
            bool sameMU = false;
            List<double> umPlan1 = new List<double>();
            List<double> umPlan2 = new List<double>();

            foreach(Beam b in p1.Beams)
            {
                if (!b.IsSetupField)
                    umPlan1.Add(b.Meterset.Value);
            }
            foreach (Beam b in p2.Beams)
            {
                if (!b.IsSetupField)
                    umPlan2.Add(b.Meterset.Value);
            }
            umPlan1.Sort();
            umPlan2.Sort();
            var result = umPlan1.Except(umPlan2);// give elements in 2 that are not in 1
            
            if(result != null) sameMU = false;
            else sameMU = true;
            return (sameMU);

        }
        public void Check()
        {

            #region QA plans
            Item_Result preparedQA = new Item_Result();
            preparedQA.Label = "CQ";
            preparedQA.ExpectedValue = "EN COURS";
            List<PlanSetup> qaPlans = new List<PlanSetup>();

            foreach(Course c in _ctx.Patient.Courses) // list QA plans of the patient
                foreach(PlanSetup p in c.PlanSetups)
                {
                    try
                    {
                        if (p.PlanIntent.ToString() == "VERIFICATION") // QA plan                        
                            qaPlans.Add(p);

                    }
                    catch
                    {
                        ; // do nothing
                    }
                    

                }

            if(_rcp.listQAplans.Count > 0) // list needed QA plans in protocol
            {                
                foreach(String qa in _rcp.listQAplans)
                {
                    bool found = false;
                    if (qa == "PDIP") // protocol wants a pdip qa
                    {

                        foreach (PlanSetup p in qaPlans)
                        {
                            if (p.Id.ToUpper().Contains("PDIP"))
                            {
                                if(haveTheSameMU(p,_ctx.PlanSetup))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found == false)
                        {

                        }
                    }
                    else if (qa == "RUBY")
                    {

                    }
                    else if (qa == "Octa4D")
                    {

                    }
                    
                    //if(haveTheSameMU(qa plan , current plan)

                }
            }
            else // no QA in protocol
            {
                preparedQA.setToINFO();
                preparedQA.MeasuredValue = "Aucun CQ attendu selon le protocole";// "Différent de Planning Approved";
                preparedQA.Infobulle = "Aucun CQ attendu selon le protocole: " + _rcp.protocolName;
            }

            this._result.Add(preparedQA);
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
