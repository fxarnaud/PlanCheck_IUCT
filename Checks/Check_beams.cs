using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using System.Windows;
using System.Windows.Navigation;

namespace PlanCheck_IUCT
{
    internal class Check_beams
    {
        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;
        private read_check_protocol _rcp;
        public Check_beams(PreliminaryInformation pinfo, ScriptContext ctx, read_check_protocol rcp)  //Constructor
        {
            _ctx = ctx;
            _pinfo = pinfo;
            _rcp = rcp;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Faisceaux";

        public void Check()
        {
            #region ENERGY 
            Item_Result energy = new Item_Result();
            energy.Label = "Energie";
            energy.ExpectedValue = "NA";

            if ((_rcp.energy == "") || (_rcp.energy == null)) // no energy specified in check-protocol
            {
                energy.setToINFO();
                energy.MeasuredValue = "Aucune énergie spécifiée dans le check-protocol " + _rcp.protocolName;
                energy.Infobulle = "Aucune énergie spécifiée dans le check-protocol " + _rcp.protocolName;
            }
            else
            {

                List<string> energyList = new List<string>();
                List<string> distinctEnergyList = new List<string>();
                foreach (Beam b in _ctx.PlanSetup.Beams)
                    if (!b.IsSetupField)
                        energyList.Add(b.EnergyModeDisplayName);

                distinctEnergyList = energyList.Distinct().ToList(); // remove doublons
                energy.MeasuredValue += "Energies : ";
                foreach (string distinctEnergy in distinctEnergyList)
                    energy.MeasuredValue += distinctEnergy + " ";
                energy.Infobulle = "Valeur spécifiée dans le check-protocol : " + _rcp.energy;
                if (distinctEnergyList.Count > 1)
                {
                    energy.setToWARNING();
                }
                else
                {
                    if (distinctEnergyList[0] == _rcp.energy)
                        energy.setToTRUE();
                    else
                        energy.setToFALSE();
                }
            }
            this._result.Add(energy);
            #endregion


            #region tolerance table
            Item_Result toleranceTable = new Item_Result();
            toleranceTable.Label = "Table de tolérance";
            toleranceTable.ExpectedValue = "NA";
    
            bool toleranceOK = true;
            List<string> listOfTolTable = new List<string>();
            String firstTT = null;
            bool firstTTfound = false;
            bool allSame = false;
            
            foreach (Beam b in _ctx.PlanSetup.Beams)
            {
                listOfTolTable.Add(b.Id + "\t(" + b.ToleranceTableLabel + ")");
                // this part is to check if the tol table are all the same
                if (!firstTTfound) 
                {
                    firstTTfound = true;
                    allSame = true;
                    firstTT = b.ToleranceTableLabel;
                }
                else
                {
                    if (b.ToleranceTableLabel != firstTT)
                        allSame = false;
                }
                // this part is to check if the tol table are as specified in schek protocol
                if (b.ToleranceTableLabel != _rcp.toleranceTable)
                {
                    toleranceOK = false;
                    
                }
            }
            if (toleranceOK)
            {
                toleranceTable.setToTRUE();
                toleranceTable.MeasuredValue = _rcp.toleranceTable;
                toleranceTable.Infobulle = "Tous les champs ont bien la table de tolérance spécifiée dans le check-protocol:\n";
            }
            else
            {
                toleranceTable.setToFALSE();
                toleranceTable.MeasuredValue = "Table de tolérances des champs à revoir (voir détail)";
                toleranceTable.Infobulle += "\n\nCertains des chams suivants n'ont pas la bonne table de tolérance\n";
                
            }
            if (_rcp.toleranceTable == "") // if no table specidfied in RCP
            {
                
                toleranceTable.MeasuredValue = "Table de tolérances unique  (voir détail) ";
                toleranceTable.Infobulle = "Pas de table de tolérance spécifiée dans le check-protocol " + _rcp.protocolName;
                if (allSame)
                {
                    toleranceTable.Infobulle += "\nUnse seule table de tolérance est utilisée pour tous les faisceaux\n";
                    toleranceTable.MeasuredValue = "Table de tolérances unique  (voir détail) ";
                    toleranceTable.setToTRUE();
                }
                else
                {
                    toleranceTable.Infobulle += "\nPlusieurs tables de tolérance utilisées pour les faisceaux\n";
                    toleranceTable.MeasuredValue = "Table de tolérances différentes  (voir détail) ";
                    toleranceTable.setToFALSE(); 
                }
                
            }
            foreach (String field in listOfTolTable)
                toleranceTable.Infobulle += "\n - " + field;

            this._result.Add(toleranceTable);
            #endregion

            #region Technique 
            /*Item_Result technique = new Item_Result();
            string myTech = null;
            // comment to be finsish 
            bool differentTech = false;
            foreach (Beam b in _ctx.PlanSetup.Beams)
                if (!b.IsSetupField)
                    if (myTech == null)
                        myTech = b.Technique.Id; // first beam technique
                    else if (myTech != b.Technique.Id)
                        differentTech = true; // check if there are several technique

            

            technique.Label = "Technique";
            technique.ExpectedValue = "NA";

            technique.setToINFO();
            technique.MeasuredValue = "pas encore de test (en cours)";// myTech;// "Différent de Planning Approved";

            technique.Infobulle = "en cours";
            this._result.Add(technique);*/
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
