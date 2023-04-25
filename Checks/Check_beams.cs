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
            if ((surfaceJaw < surfaceZX) && (surfaceJaw < surfaceZY))
                itIsTooSmall = true;
            return itIsTooSmall;

        }


        public void Check()
        {

            #region ENERGY 

            if ((!_pinfo.isTOMO) && (!_pinfo.isHALCYON)) // not checked if mono energy machine
            {
                Item_Result energy = new Item_Result();
                energy.Label = "Energie";
                energy.ExpectedValue = "NA";



                if ((_rcp.energy == "") || (_rcp.energy == null)) // no energy specified in check-protocol
                {
                    energy.setToINFO();
                    energy.MeasuredValue = "Energie non vérifiée";
                    energy.Infobulle = "Aucune énergie spécifiée dans le protocole:" + _rcp.protocolName;
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
            }
            #endregion

            #region tolerance table
            if (!_pinfo.isTOMO)
            {
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


                    listOfTolTable.Add(b.Id + "\t(" + b.ToleranceTableLabel.ToUpper() + ")");
                    // this part is to check if the tol table are all the same
                    if (!firstTTfound)
                    {
                        firstTTfound = true;
                        allSame = true;
                        firstTT = b.ToleranceTableLabel.ToUpper();
                    }
                    else
                    {
                        if (b.ToleranceTableLabel.ToUpper() != firstTT)
                            allSame = false;
                    }
                    // this part is to check if the tol table are as specified in schek protocol
                    if (b.ToleranceTableLabel.ToUpper() != _rcp.toleranceTable.ToUpper())
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

                if (_pinfo.isTOMO)
                {
                    toleranceTable.Infobulle += "\nNon vérifié pour les tomos\n";
                    toleranceTable.MeasuredValue = "Tomo (pas de table de tolérance)";
                    toleranceTable.setToINFO();
                }
                this._result.Add(toleranceTable);
            }
            #endregion

            #region FIELD SIZE GENERAL
            if (!_pinfo.isTOMO)
            {
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
                //int n = 0;
                string listOfWrongBeam = null;
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



                if (!giveup)
                {
                    foreach (Beam b in _ctx.PlanSetup.Beams)
                    {
                        if (!b.IsSetupField)
                        {


                            foreach (ControlPoint cp in b.ControlPoints)
                            {
                                if (fieldIsTooSmall(surfaceZX, surfaceZY, cp.JawPositions.X1, cp.JawPositions.X2, cp.JawPositions.Y1, cp.JawPositions.Y2))
                                {
                                    listOfWrongBeam += "\n - " + b.Id;
                                    break;
                                }
                                //                            n++;
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
                    if (listOfWrongBeam == null)
                    {
                        fieldTooSmall.setToTRUE();
                        fieldTooSmall.MeasuredValue = "Dimensions des Jaws correctes";
                        fieldTooSmall.Infobulle += "\n\nTous les champs ou Control Points ont des dimensions de machoîres cohérentes par rapport au volume cible";
                    }
                    else
                    {
                        fieldTooSmall.setToWARNING();
                        fieldTooSmall.MeasuredValue = "Un ou plusieurs champs trop petits";
                        fieldTooSmall.Infobulle += "\n\nAu moins un champ ou un Control Point a des dimensions de machoîres trop petites par rapport au volume cible" + listOfWrongBeam;
                    }


                }

                this._result.Add(fieldTooSmall);
            }
            #endregion

            #region MLC SIZE HALCYON
            if (_pinfo.isHALCYON) // if  HALCYON XxY must be < 20x20
            {
                Item_Result maxPositionMLCHalcyon = new Item_Result();
                maxPositionMLCHalcyon.Label = "Lames MLC Halcyon < 10 cm";
                maxPositionMLCHalcyon.ExpectedValue = "NA";
                maxPositionMLCHalcyon.Infobulle = "Les lames du MLC pour l'Halcyon doivent être < 100 mm (tolérance 5 mm)";

                // List<String> mlcTooLarge = new List<String>();
                double thisleafnotok = 0;
                bool allLeavesOK = true;


                //int i = 0;

                foreach (Beam b in _ctx.PlanSetup.Beams)
                {
                    if (!b.IsSetupField)
                    {
                        foreach (ControlPoint cp in b.ControlPoints)
                        {
                            foreach (double f in cp.LeafPositions)
                            {
                                if ((f > 105) || (f < -105))
                                {
                                    allLeavesOK = false; // break loop on leaves
                                    thisleafnotok = f;
                                    //MessageBox.Show("NOT GOOD " + f + " beam " + b.Id + " cp " + cp.Index.ToString());
                                    break;
                                }
                            }

                            if (!allLeavesOK)
                            {

                                break; // break loop on cp
                            }
                        }
                        // +" "+ cp.JawPositions.X1.ToString()  +" " +cp.JawPositions.X2.ToString()+" ");

                        if (!allLeavesOK)
                        {
                            break; // break beam loop
                        }
                    }
                }


                // if (mlcTooLarge.Count > 0)
                if (!allLeavesOK)
                {
                    //MessageBox.Show("i = " + i.ToString());
                    maxPositionMLCHalcyon.setToWARNING();
                    maxPositionMLCHalcyon.MeasuredValue = "Au moins une lame MLC > 100 mm (" + thisleafnotok + ")";
                }
                else
                {
                    maxPositionMLCHalcyon.setToTRUE();
                    maxPositionMLCHalcyon.MeasuredValue = "Toutes les lames MLC < 100 mm";
                }
                this._result.Add(maxPositionMLCHalcyon);

            }

            #endregion

            #region NOVA SBRT 
            if (_pinfo.isNOVA)
            {
                Item_Result novaSBRT = new Item_Result();
                novaSBRT.Label = "NOVA SBRT ou NOVA";
                novaSBRT.MeasuredValue = _pinfo.machine;
                if (_pinfo.treatmentType == "VMAT")
                {

                    
                    novaSBRT.Infobulle = "Pour les Nova en VMAT, la machine NOVA SBRT doit être utilisée pour les champs < 7x7 cm2";
                    Beam b = _ctx.PlanSetup.Beams.FirstOrDefault(x => x.IsSetupField == false);
                    ControlPoint cp = b.ControlPoints.First();
                    double meanJawsXY = 0.5 * (Math.Abs(cp.JawPositions.X1) + Math.Abs(cp.JawPositions.X2)) + (Math.Abs(cp.JawPositions.Y1) + Math.Abs(cp.JawPositions.Y2));
                    double limit = 70.0;

                    if (_pinfo.machine == "NOVA SBRT")
                    {
                        novaSBRT.MeasuredValue = "NOVA SBRT (jaws moy. = " + meanJawsXY + ")";
                        if (meanJawsXY < limit)
                        {
                            novaSBRT.setToTRUE();
                        }
                        else
                        {
                            novaSBRT.setToFALSE();
                        }

                    }
                    else
                    {
                        novaSBRT.MeasuredValue = "NOVA (jaws moy. = " + meanJawsXY + ")";
                        if (meanJawsXY < limit)
                        {
                            novaSBRT.setToFALSE();
                        }
                        else
                        {
                            novaSBRT.setToTRUE();
                        }

                    }                    
                }
                else
                {
                    novaSBRT.Infobulle = "Nova non VMAT : machine NOVA SBRT interdite";
                    if (_pinfo.machine == "NOVA SBRT")
                    {
                        novaSBRT.setToFALSE();
                    }
                    else
                    {
                        novaSBRT.setToTRUE();
                    }
                }
                this._result.Add(novaSBRT);
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
