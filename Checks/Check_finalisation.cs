using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using System.Windows;


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



            double umplan1 = 0.0;
            double umplan2 = 0.0;

            foreach (Beam b in p1.Beams)
            {
                if (!b.IsSetupField)
                    umplan1 += b.Meterset.Value;
            }
            foreach (Beam b in p2.Beams)
            {
                if (!b.IsSetupField)
                    umplan2 += b.Meterset.Value;
            }


            if (umplan1 != umplan2) return (false);
            else return (true);

        }
        public void Check()
        {

            #region QA plans
            Item_Result preparedQA = new Item_Result();
            preparedQA.Label = "CQ";
            preparedQA.ExpectedValue = "EN COURS";
            String nameOfMatch = null;
            List<PlanSetup> qaPlans = new List<PlanSetup>();
            List<String> qaPlansPresent = new List<String>();
            List<String> qaPlansMissing = new List<String>();
            List<String> unapprovedQAplans = new List<String>();
            foreach (Course c in _ctx.Patient.Courses) // list QA plans of the patient
            {
                foreach (PlanSetup p in c.PlanSetups)
                {
                    try
                    {
                        if (p.PlanIntent.ToString() == "VERIFICATION") // QA plan                        
                            qaPlans.Add(p);

                       // MessageBox.Show("making the list " + p.PlanIntent.ToString());
                    }
                    catch
                    {
                        ; // do nothing
                    }
                }
            }

            if (_rcp.listQAplans.Count > 0) // list needed QA plans in protocol
            {
                foreach (String qa in _rcp.listQAplans) // loop on required QAplans
                {
                    bool found = false;
                    if (qa == "PDIP") // protocol wants a pdip qa
                    {

                       // MessageBox.Show("LOOK FOR PDIP ");
                        foreach (PlanSetup p in qaPlans) // loop on present QA plans
                        {
                           // MessageBox.Show("find " + p.Id);
                            if (p.Id.ToUpper().Contains("PDIP")||(p.Course.Id.ToUpper().Contains("PDIP")))
                            {
                             //   MessageBox.Show("well it is pdip " + p.Id);

                                if (haveTheSameMU(p, _ctx.PlanSetup))
                                {
                               //     MessageBox.Show("same UM");

                                    nameOfMatch = p.Id;
                                    found = true;
                                    if (p.ApprovalStatus.ToString() != "PlanningApproved")
                                        unapprovedQAplans.Add(p.Id);

                                    break;
                                }
                            }
                        }

                    }
                    else if (qa == "RUBY")
                    {
                        foreach (PlanSetup p in qaPlans) // loop on present QA plans
                        {
                            if (p.Id.ToUpper().Contains("RUBY") || (p.Course.Id.ToUpper().Contains("RUBY")))
                            {
                                if (haveTheSameMU(p, _ctx.PlanSetup))
                                {
                                    nameOfMatch = p.Id;
                                    found = true;
                                    if (p.ApprovalStatus.ToString() != "PlanningApproved")
                                        unapprovedQAplans.Add(p.Id);
                                    break;
                                }
                            }
                        }
                    }
                    else if (qa == "Octa4D")
                    {
                        foreach (PlanSetup p in qaPlans) // loop on present QA plans
                        {
                            if (p.Id.ToUpper().Contains("OCTA4D") || (p.Course.Id.ToUpper().Contains("OCTA4D")))
                            {
                                if (haveTheSameMU(p, _ctx.PlanSetup))
                                {
                                    nameOfMatch = p.Id;
                                    found = true;
                                    if (p.ApprovalStatus.ToString() != "PlanningApproved")
                                        unapprovedQAplans.Add(p.Id);
                                    break;
                                }
                            }
                        }
                    }

                    if (found == true)
                    {
                        qaPlansPresent.Add(qa + " --> " + nameOfMatch);
                    }
                    else
                    {
                        qaPlansMissing.Add(qa);
                    }

                }

                if (qaPlansMissing.Count > 0)
                {
                    preparedQA.setToFALSE();
                    preparedQA.MeasuredValue = "Au moins un CQ absent";// "Différent de Planning Approved";
                    preparedQA.Infobulle = "Au moins un plan CQ absent alors qu'il est requis selon le check-protocole" + _rcp.protocolName;

                }
                else if (unapprovedQAplans.Count > 0)
                {
                    preparedQA.MeasuredValue = "Plan CQ présents mais non approuvé";
                    preparedQA.Infobulle = "Tous les plans CQ requis sont présents mais au moins un n'est pas approuvé :\n";
                    foreach (String s in unapprovedQAplans)
                        preparedQA.Infobulle += "\n - " + s;
                   preparedQA.setToWARNING();
                }
                else
                {
                    preparedQA.setToTRUE();
                    preparedQA.MeasuredValue = "Tous les CQ sont présents";// "Différent de Planning Approved";
                    preparedQA.Infobulle = "Les plans CQ requis selon le check-protocole" + _rcp.protocolName + " sont présents";
                }
                if (qaPlansPresent.Count() > 0)
                {
                    preparedQA.Infobulle += "\n\nListe des plans CQ requis et présents :";
                    foreach (String s in qaPlansPresent)
                        preparedQA.Infobulle += "\n - " + s;
                }

                if (qaPlansMissing.Count() > 0)
                {
                    preparedQA.Infobulle += "\n\nListe des plans CQ requis mais absents  :";
                    foreach (String s in qaPlansMissing)
                        preparedQA.Infobulle += "\n - " + s;
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
