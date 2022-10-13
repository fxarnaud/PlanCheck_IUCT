using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanCheck_IUCT
{
    internal class Check_Algorithm
    {
        public Check_Algorithm(PlanInformation pinfo)  //Constructor
        {
            // _testpartlabel = "Algorithme";
            _pinfo = pinfo;
            Check();
        }

        private List<Item_Result> _result = new List<Item_Result>();
        private PlanInformation _pinfo;
        private string _title = "Algorithme";

        public void Check()
        {
            Comparator testing = new Comparator();

            //Nom de l'algo
            Item_Result algo_name = new Item_Result();
            algo_name.Label = "Algorithme de calcul";
            algo_name.ExpectedValue = "AAA_13714";
            algo_name.MeasuredValue = _pinfo.AlgoName;
            algo_name.Comparator = "=";
            algo_name.Infobulle = "Infobulle de l'algo de clacul"; 
            algo_name.ResultStatus = testing.CompareDatas(algo_name.ExpectedValue, algo_name.MeasuredValue, algo_name.Comparator);
            this._result.Add(algo_name);


            //Grille de resolution
            Item_Result algo_grid = new Item_Result();
            algo_grid.Label = "Taille grille de calcul";
            algo_grid.ExpectedValue = "0.125";
            algo_grid.MeasuredValue = "0.125";
            algo_grid.Comparator = "=";
            algo_grid.Infobulle = "Infobulle de la taille grille de calcul";
            algo_grid.ResultStatus = testing.CompareDatas(algo_grid.ExpectedValue, algo_grid.MeasuredValue, algo_grid.Comparator);
            this._result.Add(algo_grid);
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
