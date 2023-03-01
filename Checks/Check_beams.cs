using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using System.Windows;
using System.Windows.Navigation;


namespace PlanCheck
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


        private bool fieldIsTooSmall(double surfaceZX, double surfaceZY, double X1, double X2, double Y1, double Y2)
        {
            bool itIsTooSmall = false;
            double tolerance = 1.2;
            double surfaceJaw = tolerance * (Math.Abs(X1) + Math.Abs(X2)) * (Math.Abs(Y1) + Math.Abs(Y2));
            if ((surfaceJaw < surfaceZX) || (surfaceJaw < surfaceZY))
                itIsTooSmall = true;
            return itIsTooSmall;

        }

       
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


            #region FIELD SIZE GENERAL
            String machine = _ctx.PlanSetup.Beams.FirstOrDefault().TreatmentUnit.Id;
            bool giveup = false;
            Item_Result fieldTooSmall = new Item_Result();
            List<String> fieldTooSmallList = new List<String>();
            fieldTooSmall.Label = "Champs trop petits";
            fieldTooSmall.ExpectedValue = "NA";
            fieldTooSmall.Infobulle = "Les champs doivent avoir une dimension adaptée au PTV";
            String targetName = _ctx.PlanSetup.TargetVolumeID;
            Structure target = null;
            double surfaceZX = 0;
            double surfaceZY = 0;
            int n = 0;
            try // do we have a target volume ? 
            {
                target = _ctx.StructureSet.Structures.Where(s => s.Id == targetName).FirstOrDefault();
                surfaceZX = target.MeshGeometry.Bounds.SizeZ * target.MeshGeometry.Bounds.SizeX;
                surfaceZY = target.MeshGeometry.Bounds.SizeZ * target.MeshGeometry.Bounds.SizeY;
            }
            catch // no we don't
            {
                giveup = true;
            }

            if (machine.Contains("TOM")) // if  not Tomo
                giveup = true;

            if (!giveup)
            {
                foreach (Beam b in _ctx.PlanSetup.Beams)
                {
                    if (!b.IsSetupField)
                    {

                        
                        foreach (ControlPoint cp in b.ControlPoints)
                        {
                            if (fieldIsTooSmall(surfaceZX, surfaceZY, cp.JawPositions.X1, cp.JawPositions.X2, cp.JawPositions.Y1, cp.JawPositions.Y2))
                                n++;
                        }
                    }
                }
            }


            if (giveup)
            {
                fieldTooSmall.setToINFO();
                fieldTooSmall.MeasuredValue = "Test non réalisé";
                fieldTooSmall.Infobulle += "\n\nCe test n'est pas réalisé pour les Tomos ou si le plan n'a pas de volume cible";
            }
            else
            {
                if(n==0)
                {
                    fieldTooSmall.setToTRUE();
                    fieldTooSmall.MeasuredValue = "Dimensions des Jaws correctes";
                    fieldTooSmall.Infobulle += "\n\nTous les champs ou Control Points ont des dimensions de machoîres cohérentes par rapport au volume cible";
                }
                else
                {
                    fieldTooSmall.setToWARNING();
                    fieldTooSmall.MeasuredValue = "Un ou plusieurs champs trop petits";
                    fieldTooSmall.Infobulle += "\n\nAu moins un champ ou un Control Point a des dimensions de machoîres trop petites par rapport au volume cible";
                }


            }

            this._result.Add(fieldTooSmall);
            #endregion

            #region FIELD SIZE HALCYON
            Item_Result fieldTooLargeHalcyon = new Item_Result();
            fieldTooLargeHalcyon.Label = "Champs Halcyon > 20x20";
            fieldTooLargeHalcyon.ExpectedValue = "NA";
            fieldTooLargeHalcyon.Infobulle = "Les machoîres Halcyon doivent être < 10 cm";

            List<String> fieldTooLarge = new List<String>();

            
            if (machine.Contains("HALCYON")) // if  HALCYON XxY must be < 20x20
            {
                foreach (Beam b in _ctx.PlanSetup.Beams)
                {
                    if (!b.IsSetupField)
                        foreach (ControlPoint cp in b.ControlPoints)
                            if ((cp.JawPositions.X1 > 10.0) || (cp.JawPositions.X2 > 10.0) || (cp.JawPositions.Y1 > 10.0) || (cp.JawPositions.Y2 > 10.0))
                                fieldTooLarge.Add(b.Id);
                }


                if (fieldTooLarge.Count > 0)
                {
                    fieldTooLargeHalcyon.setToFALSE();
                    fieldTooLargeHalcyon.MeasuredValue = fieldTooLarge.Count + " Control Point(s) hors limite";
                }
                else
                {
                    fieldTooLargeHalcyon.setToTRUE();
                    fieldTooLargeHalcyon.MeasuredValue = " Aucun Control Point hors limite";
                }
                this._result.Add(fieldTooLargeHalcyon);

            }

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
