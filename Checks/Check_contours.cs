using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck_IUCT
{
    internal class Check_contours
    {
        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;

        public Check_contours(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {
            _ctx = ctx;
            _pinfo = pinfo;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Contours (en cours)";

        public void Check()
        {

            #region A FAIRE ? 
            Item_Result approve = new Item_Result();
            approve.Label = "en cours";
            approve.ExpectedValue = "EN COURS";

            approve.setToINFO();
            approve.MeasuredValue = "pas encore de test (en cours)";// "Différent de Planning Approved";

            approve.Infobulle = "en cours";
            this._result.Add(approve);
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
