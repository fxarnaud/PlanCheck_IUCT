﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Runtime.CompilerServices;
using System.Reflection;
using PlanCheck;
using PlanCheck.Users;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Navigation;
using System.Drawing;




namespace PlanCheck
{
    internal class Check_Model
    {
        public Check_Model(PreliminaryInformation pinfo, ScriptContext context, read_check_protocol rcp)  //Constructor
        {
            // _testpartlabel = "Algorithme";
            _rcp = rcp;
            _pinfo = pinfo;
            _pcontext = context;
            Check();
        }

        public bool CompareNTO(OptimizationNormalTissueParameter planNTO, NTO protocolNTO)
        {
            bool result = true;

            if (planNTO.DistanceFromTargetBorderInMM != protocolNTO.distanceTotTarget) result = false;
            if (planNTO.StartDosePercentage != protocolNTO.startPercentageDose) result = false;
            if (planNTO.EndDosePercentage != protocolNTO.stopPercentageDose) result = false;
            if (planNTO.FallOff != protocolNTO.theFalloff) result = false;
            if (planNTO.Priority != protocolNTO.priority) result = false;

            bool autoMode = false;
            if (protocolNTO.mode == "Manual") autoMode = false;
            else if (protocolNTO.mode == "Auto") autoMode = true;

            if (planNTO.IsAutomatic != autoMode) result = false;

            return result;



        }
        private List<Item_Result> _result = new List<Item_Result>();
        private PreliminaryInformation _pinfo;
        private ScriptContext _pcontext;
        private string _title = "Modèle de calcul";
        private read_check_protocol _rcp;
        public void Check()
        {

            String machin = _pcontext.PlanSetup.Beams.First().TreatmentUnit.Id;
            bool isTomo = false;
            if (machin.Contains("TOM"))
                isTomo = true;

            Comparator testing = new Comparator();


            #region Nom de l'algo
            Item_Result algo_name = new Item_Result();
            algo_name.Label = "Algorithme de calcul";
            algo_name.ExpectedValue = _rcp.algoName;
            algo_name.MeasuredValue = _pinfo.AlgoName;
            algo_name.Comparator = "=";
            algo_name.Infobulle = "Algorithme attendu pour le check-protocol " + _rcp.protocolName + " : " + algo_name.ExpectedValue;
            algo_name.Infobulle += "\nLes options de calcul ne sont pas vérifiées si l'algorithme n'est pas celui attendu";
            algo_name.ResultStatus = testing.CompareDatas(algo_name.ExpectedValue, algo_name.MeasuredValue, algo_name.Comparator);
            this._result.Add(algo_name);
            #endregion

            #region Grille de resolution
            Item_Result algo_grid = new Item_Result();
            algo_grid.Label = "Taille grille de calcul (mm)";
            algo_grid.ExpectedValue = _rcp.gridSize.ToString();//"1.25";// TO GET IN PRTOCOLE
            algo_grid.MeasuredValue = _pcontext.PlanSetup.Dose.XRes.ToString("0.00");
            //algo_grid.Comparator = "=";
            algo_grid.Infobulle = "Grille de calcul attendue pour le check-protocol " + _rcp.protocolName + " " + algo_grid.ExpectedValue + " mm";

            //algo_grid.ResultStatus = testing.CompareDatas(algo_grid.ExpectedValue, algo_grid.MeasuredValue, algo_grid.Comparator);
            if (_rcp.gridSize == _pcontext.PlanSetup.Dose.XRes)
            {
                algo_grid.setToTRUE();
            }
            else
            {
                algo_grid.setToFALSE();
            }
            if (isTomo)
            {
                if (_pcontext.PlanSetup.Dose.XRes - 1.2695 < 0.01)
                {
                    algo_grid.setToTRUE();
                }
                else
                {
                    algo_grid.setToFALSE();
                }
                algo_grid.Infobulle = "Pour les tomos, la grille doit être 1.27 mm (check protocol ignoré)";

            }

            this._result.Add(algo_grid);
            #endregion



            #region LES OPTIONS DE CALCUL
            if (algo_name.ResultStatus.Item1 != "X")// options are not checked if the algo is not the same
            {
                Item_Result options = new Item_Result();
                options.Label = "Autres options du modèle de calcul";

                options.ExpectedValue = "N/A";// TO GET IN PRTOCOLE

                options.Comparator = "=";

                int optionsAreOK = 1;
                int myOpt = 0;

                foreach (string s in _pinfo.Calculoptions)
                {
                    if (s != _rcp.optionComp[myOpt]) // if one computation option is different test is error
                    {
                        options.Infobulle = "Une option de calcul est différente du check-protocol " + _rcp.protocolName;
                        options.MeasuredValue = s + " (options de calcul du plan) vs. " + _rcp.optionComp[myOpt] + " (attendu pour ce check-protocol) ";
                        optionsAreOK = 0;
                    }
                    myOpt++;
                }

                if (optionsAreOK == 0)
                {
                    options.setToFALSE();
                }
                else
                {
                    options.setToTRUE();
                    options.Infobulle = "Les " + myOpt + " options du modèle calcul sont en accord avec le check-protocol: " + _rcp.protocolName;
                    options.MeasuredValue = "OK";

                }

                this._result.Add(options);
            }
            #endregion

            #region NTO



            if (_pcontext.PlanSetup.OptimizationSetup.Parameters.Count() > 0) // if there is an optim. pararam
            {
                //foreach (OptimizationObjective oo in _ctx.PlanSetup.OptimizationSetup.Objectives)
                // foreach (OptimizationParameter op in _ctx.PlanSetup.OptimizationSetup.Parameters)




                OptimizationNormalTissueParameter ontp = _pcontext.PlanSetup.OptimizationSetup.Parameters.FirstOrDefault(x => x.GetType().Name == "OptimizationNormalTissueParameter") as OptimizationNormalTissueParameter;

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
                NTO.Infobulle += "\n Fall off : " + _rcp.NTOparams.theFalloff;
                NTO.Infobulle += "\n Start Dose : " + _rcp.NTOparams.startPercentageDose;
                NTO.Infobulle += "\n End Dose : " + _rcp.NTOparams.stopPercentageDose;
                NTO.Infobulle += "\n Priority : " + _rcp.NTOparams.priority;
                NTO.Infobulle += "\n Auto Mode : " + _rcp.NTOparams.mode;
                this._result.Add(NTO);


                /*
                OptimizationExcludeStructureParameter oesp = op as OptimizationExcludeStructureParameter;
                OptimizationIMRTBeamParameter oibp = op as OptimizationIMRTBeamParameter;
                
                OptimizationPointCloudParameter opcp = op as OptimizationPointCloudParameter;

                */


            }


            #endregion
            #region Jaw tracking
            //  This method doesnt work:
            //  OptimizationJawTrackingUsedParameter ojtup = op as OptimizationJawTrackingUsedParameter;
            //  (found on the reddit )

            if (_pcontext.PlanSetup.OptimizationSetup.Parameters.Count() > 0) // if there is an optim. pararam
            {
                Item_Result jawTrack = new Item_Result();
                jawTrack.Label = "Jaw Track";
                //OptimizationJawTrackingUsedParameter ojtup = _ctx.PlanSetup.OptimizationSetup.Parameters.FirstOrDefault(x => x.GetType().Name == "OptimizationJawTrackingUsedParameter") as OptimizationJawTrackingUsedParameter;
                jawTrack.Infobulle = "Selon le protocole " + _rcp.protocolName + " le jaw tracking doit être " + _rcp.JawTracking;

                bool isJawTrackingOn = _pcontext.PlanSetup.OptimizationSetup.Parameters.Any(x => x is OptimizationJawTrackingUsedParameter);
                jawTrack.MeasuredValue = isJawTrackingOn.ToString();

                if (isJawTrackingOn != _rcp.JawTracking)
                {
                    jawTrack.setToFALSE();
                }
                else
                {
                    jawTrack.setToTRUE();
                }
                this._result.Add(jawTrack);
            }
            #endregion


            #region LES OPTIONS DU PO
            if (algo_name.ResultStatus.Item1 != "X")// options are not checked if the algo is not the same
            {
                Item_Result POoptions = new Item_Result();
                POoptions.Label = "Options du PO";

                POoptions.ExpectedValue = "N/A";// TO GET IN PRTOCOLE




                int myOpt = 0;
                
                bool optionsPOareOK = true;
                foreach (string s in _rcp.POoptions)
                {
                    //                    MessageBox.Show("Comp " + s + " vs. " + _pinfo.POoptions[myOpt]);
                    if (s != _pinfo.POoptions[myOpt])
                        optionsPOareOK = false;
                    myOpt++;
                }

                if (!optionsPOareOK)
                {
                    POoptions.setToFALSE();
                    POoptions.MeasuredValue = "Option(s) du PO non conforme au protocole (voir détail)";
                    POoptions.Infobulle = "Une option du PO est différente du check-protocol " + _rcp.protocolName;
                    myOpt = 0;
                    foreach (string s in _rcp.POoptions)
                    {
                        POoptions.Infobulle += s + " vs. " + _pinfo.POoptions[myOpt] + "\n";
                        myOpt++;
                    }
                }
                else
                {
                    POoptions.setToTRUE();
                    POoptions.Infobulle = "Les " + myOpt + " options du modèle PO sont en accord avec le check-protocol: " + _rcp.protocolName;
                    POoptions.MeasuredValue = "OK";

                }

                this._result.Add(POoptions);
            }
            #endregion








        }



        //_pcontext.PlanSetup.PhotonCalculationOptions
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
