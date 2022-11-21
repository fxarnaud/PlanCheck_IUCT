using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Runtime.CompilerServices;
using System.Reflection;
using PlanCheck_IUCT;
using PlanCheck_IUCT.Users;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Navigation;
using System.Drawing;




namespace PlanCheck_IUCT
{
    internal class Check_Plan
    {
        public Check_Plan(PreliminaryInformation pinfo, ScriptContext context, read_check_protocol rcp)  //Constructor
        {
            // _testpartlabel = "Algorithme";
            _rcp = rcp;
            _pinfo = pinfo;
            _pcontext = context;
            Check();
        }

        private List<Item_Result> _result = new List<Item_Result>();
        private PreliminaryInformation _pinfo;
        private ScriptContext _pcontext;
        private string _title = "Plan";
        private read_check_protocol _rcp;
        public void Check()
        {
            Comparator testing = new Comparator();

            #region Nom de l'algo
            Item_Result algo_name = new Item_Result();
            algo_name.Label = "Algorithme de calcul";
            algo_name.ExpectedValue = _rcp.algoName;
            algo_name.MeasuredValue = _pinfo.AlgoName;
            algo_name.Comparator = "=";
            algo_name.Infobulle = "Algorithme attendu pour le protocole " + _rcp.protocolName + " : " + algo_name.ExpectedValue;
            algo_name.ResultStatus = testing.CompareDatas(algo_name.ExpectedValue, algo_name.MeasuredValue, algo_name.Comparator);
            this._result.Add(algo_name);
            #endregion

            #region Grille de resolution
            Item_Result algo_grid = new Item_Result();
            algo_grid.Label = "Taille grille de calcul (mm)";
            algo_grid.ExpectedValue = _rcp.gridSize.ToString();//"1.25";// TO GET IN PRTOCOLE
            algo_grid.MeasuredValue = _pcontext.PlanSetup.Dose.XRes.ToString();
            algo_grid.Comparator = "=";
            algo_grid.Infobulle = "Grille de calcul attendue pour le protocole " + _rcp.protocolName + " " + algo_grid.ExpectedValue + " mm";
            algo_grid.ResultStatus = testing.CompareDatas(algo_grid.ExpectedValue, algo_grid.MeasuredValue, algo_grid.Comparator);
            this._result.Add(algo_grid);
            #endregion

            #region LES OPTIONS DE CALCUL
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
                    options.Infobulle = "Une option de calcul est différente du protocole " + _rcp.protocolName;
                    options.MeasuredValue = s + " (options de calcul du plan) vs. " + _rcp.optionComp[myOpt]+ " (attendu pour ce protocole) ";
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
                options.Infobulle = "Les " + myOpt + " options du modèle calcul sont en accord avec le protocole: " + _rcp.protocolName;
                options.MeasuredValue = "OK";

            }

            this._result.Add(options);
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
