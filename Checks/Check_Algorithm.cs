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
    internal class Check_Algorithm
    {
        public Check_Algorithm(PreliminaryInformation pinfo, ScriptContext context)  //Constructor
        {
            // _testpartlabel = "Algorithme";
            _pinfo = pinfo;
            _pcontext = context;
            Check();
        }

        private List<Item_Result> _result = new List<Item_Result>();
        private PreliminaryInformation _pinfo;
        private ScriptContext _pcontext;
        private string _title = "Algorithme";

        public void Check()
        {
            Comparator testing = new Comparator();

            #region Nom de l'algo
            Item_Result algo_name = new Item_Result();
            algo_name.Label = "Algorithme de calcul";
            algo_name.ExpectedValue = "AAA_15605New";// TO GET IN PRTOCOLE
            algo_name.MeasuredValue = _pinfo.AlgoName;
            algo_name.Comparator = "=";
            algo_name.Infobulle = "Algorithme attendu pour ce protocole : " + algo_name.ExpectedValue;
            algo_name.ResultStatus = testing.CompareDatas(algo_name.ExpectedValue, algo_name.MeasuredValue, algo_name.Comparator);
            this._result.Add(algo_name);
            #endregion

            #region Grille de resolution
            Item_Result algo_grid = new Item_Result();
            algo_grid.Label = "Taille grille de calcul";
            algo_grid.ExpectedValue = "1.25";// TO GET IN PRTOCOLE
            algo_grid.MeasuredValue = _pcontext.PlanSetup.Dose.XRes.ToString();
            algo_grid.Comparator = "=";
            algo_grid.Infobulle = "Grille de calcul attendue pour ce protocole : " + algo_grid.ExpectedValue;
            algo_grid.ResultStatus = testing.CompareDatas(algo_grid.ExpectedValue, algo_grid.MeasuredValue, algo_grid.Comparator);
            this._result.Add(algo_grid);
            #endregion

            #region A FAIRE LES OPTIONS DE CALCUL
            Item_Result options = new Item_Result();
            options.Label = "Options de calcul (EN COURS)";
            options.ExpectedValue = "1.25";// TO GET IN PRTOCOLE
            options.MeasuredValue = "EN COURS";
            options.Comparator = "=";
            options.Infobulle = "en cours..." + options.ExpectedValue;
            //            options.ResultStatus = testing.CompareDatas(options.ExpectedValue, options.MeasuredValue, options.Comparator);
            options.setToTRUE();
            this._result.Add(options);
            /* foreach (string s in _pinfo.Calculoptions)
             {
                 MessageBox.Show(s);
             }*/
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
