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
        private string _title = "Faisceaux (en cours)";

        public void Check()
        {

            #region Technique 
            Item_Result technique = new Item_Result();
            //string myTech = null;
            /* comment to be finsish 
            bool differentTech = false;
            foreach (Beam b in _ctx.PlanSetup.Beams)
                if (!b.IsSetupField)
                    if (myTech == null)
                        myTech = b.Technique.Id; // first beam technique
                    else if (myTech != b.Technique.Id)
                        differentTech = true; // check if there are several technique

            */

            technique.Label = "Technique";
            technique.ExpectedValue = "NA";

            technique.setToINFO();
            technique.MeasuredValue = "pas encore de test (en cours)";// myTech;// "Différent de Planning Approved";

            technique.Infobulle = "en cours";
            this._result.Add(technique);
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
