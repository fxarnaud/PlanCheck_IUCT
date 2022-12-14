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
    internal class Check_UM
    {
        private ScriptContext _ctx;

        public Check_UM(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {
            _ctx = ctx;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "UM";

        public void Check()
        {
           

            #region UM per Gray
            Item_Result um = new Item_Result();
            um.Label = "UM";
            um.ExpectedValue = "EN COURS";
            double n_um = 0.0;
            double n_um_per_gray = 0.0;
            String myMLCType = null;
            int uncorrFieldWithaWedge = 0;
            int FieldWithLessThan10UM = 0;
            foreach (Beam b in _ctx.PlanSetup.Beams)
            {
                if (!b.IsSetupField)
                {
                    if (b.Meterset.Value < 20.0)
                        if (b.Wedges.Count() > 0)
                            uncorrFieldWithaWedge++;
                    if (b.Meterset.Value < 9.5)
                    {
                        FieldWithLessThan10UM++;
                        //MessageBox.Show(b.Id + b.Meterset.Value.ToString());
                    }
                    myMLCType = b.MLCPlanType.ToString();
                    n_um = n_um + Math.Round(b.Meterset.Value, 1);
                }
            }


            n_um_per_gray = n_um / (_ctx.PlanSetup.DosePerFraction.Dose / _ctx.PlanSetup.TreatmentPercentage);
            n_um_per_gray = Math.Round(n_um_per_gray / 100, 3);
            um.MeasuredValue = n_um.ToString() + " UM (" + n_um_per_gray + " UM/cGy)";

           // MessageBox.Show(n_um_per_gray.ToString("N2") + myMLCType);

            if ((myMLCType == "VMAT") || (myMLCType == "IMRT"))
            {
                um.setToTRUE();
                if (n_um_per_gray > 3.5)
                    um.setToWARNING();
                
                if (n_um_per_gray > 4.5)
                    um.setToFALSE();

                
            }
            else
            {
                if (n_um_per_gray > 1.5)
                    um.setToFALSE();
                else
                    um.setToTRUE();

            }




            um.Infobulle = "Le nombre d'UM par cGy doit être < 1.5 en RT. En VMAT/IMRT warning si > 3.5 et ERREUR si > 4.5. A noter que pour Halcyon pelvis on accepte < 4.5 et pour les RA vertebre < 5";


            // um.Infobulle = thereIsAFieldWithaWedge;


            this._result.Add(um);
            #endregion

            #region UM Champs filtrés ?
            Item_Result wedged = new Item_Result();
            wedged.Label = "Champs filtrés";
            wedged.ExpectedValue = "EN COURS";

            if (uncorrFieldWithaWedge != 0)
            {
                wedged.MeasuredValue = uncorrFieldWithaWedge.ToString() + " champs filtrés avec < 20 UM";
                wedged.setToFALSE();
                wedged.Infobulle = uncorrFieldWithaWedge.ToString() +  " champs filtrés avec moins de 20 UM";

            }
            else
            {
                wedged.MeasuredValue = "OK";
                wedged.setToTRUE();
                wedged.Infobulle = "Pas de champs filtré avec moins de 20 UM";

            }

            this._result.Add(wedged);
            #endregion


            #region Champs < 10 UM ?
            Item_Result less10UM = new Item_Result();
            less10UM.Label = "Champs < 10 UM";
            less10UM.ExpectedValue = "EN COURS";

            if (FieldWithLessThan10UM != 0)
            {
                less10UM.MeasuredValue = FieldWithLessThan10UM.ToString() + " champs avec < 10 UM";
                less10UM.setToFALSE();
                less10UM.Infobulle = FieldWithLessThan10UM.ToString() + " champs avec < 10 UM";

            }
            else
            {
                less10UM.MeasuredValue = "OK";
                less10UM.setToTRUE();
                less10UM.Infobulle = "Pas de champs < 10 UM";

            }

            this._result.Add(less10UM);
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
