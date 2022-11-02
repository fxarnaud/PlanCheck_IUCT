using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck_IUCT
{
    internal class Check_Beam
    {
        private ScriptContext _ctx;

        public Check_Beam(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {
            _ctx = ctx;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Faisceaux";

        public void Check()
        {
            #region Plan approuvé ?
            Item_Result approve = new Item_Result();
            approve.Label = "Statut d'approbation du plan";
            approve.ExpectedValue = "EN COURS";

            if (_ctx.PlanSetup.ApprovalStatus.ToString() == "PlanningApproved")
            {
                approve.MeasuredValue = "Plan approuvé ";// + _ctx.PlanSetup.PlanningApprover;
                approve.setToTRUE();
            }
            else
            {
                approve.MeasuredValue = "Différent de Planning Approved";
                approve.setToFALSE();
            }
            approve.Infobulle = "Le plan doit être Planning Approved";
            this._result.Add(approve);
            #endregion

            #region UM per Gray
            Item_Result um = new Item_Result();
            um.Label = "UM";
            um.ExpectedValue = "EN COURS";
            double n_um = 0.0;
            double n_um_per_gray = 0.0;
            String myMLCType=null;
            String thereIsAFieldWithaWedge = null;
            foreach (Beam b in _ctx.PlanSetup.Beams)
            {
                if (!b.IsSetupField)
                {
                    if (b.Meterset.Value < 20.0)
                        if (b.Wedges.Count() > 0)
                            thereIsAFieldWithaWedge = b.Id;

                    myMLCType = b.MLCPlanType.ToString();                    
                    n_um = n_um + Math.Round(b.Meterset.Value, 1);               
                }
            }


            n_um_per_gray = n_um / (_ctx.PlanSetup.DosePerFraction.Dose /  _ctx.PlanSetup.TreatmentPercentage);
            n_um_per_gray = Math.Round(n_um_per_gray/100, 3);
            um.MeasuredValue = n_um.ToString() + " UM ("+ n_um_per_gray + " UM/cGy)";


            if (myMLCType == "DoseDynamic")
            {

                if (n_um_per_gray > 3.5)
                    um.setToFALSE();
                else
                    um.setToTRUE();
            }
            else
            {
                if (n_um_per_gray > 1.5)
                    um.setToFALSE();
                else
                    um.setToTRUE();

            }

            


            um.Infobulle = "Le nombre d'UM par cGy doit être < 1.5 en RT, < 3.5 en VMAT. A noter que pour H8 pelvis on accepte < 4.5 et pour les RA vertebre < 5";

           
           // um.Infobulle = thereIsAFieldWithaWedge;


            this._result.Add(um);
            #endregion

            #region Champs filtrés ?
            Item_Result wedged = new Item_Result();
            wedged.Label = "Champs filtrés";
            wedged.ExpectedValue = "EN COURS";

            if(thereIsAFieldWithaWedge != null)
            {
                wedged.MeasuredValue = thereIsAFieldWithaWedge + " < 25 UM";
                wedged.setToFALSE();
                wedged.Infobulle = "Au moins un champs filtrés a moins de 25 UM ("+ thereIsAFieldWithaWedge+")";

            }
            else
            {
                wedged.MeasuredValue = "OK";
                wedged.setToTRUE();
                wedged.Infobulle = "Pas de champs filtré avec moins de 25 UM";

            }
            
            this._result.Add(wedged);
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
