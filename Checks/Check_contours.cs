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
        private read_check_protocol _rcp;

        public Check_contours(PreliminaryInformation pinfo, ScriptContext ctx,read_check_protocol rcp)  //Constructor
        {
            _rcp = rcp;
            _ctx = ctx;
            _pinfo = pinfo;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Contours (en cours)";

        public void Check()
        {

            #region COUCH STRUCTURES 
            Item_Result couchStructExist = new Item_Result();
            couchStructExist.Label = "Structures de table";
            couchStructExist.ExpectedValue = "EN COURS";
            
               


            
//  .Where(w => w.StartsWith("a"))


            foreach (String s in _rcp.couchStructInProtocol) /// list of couch structures un protocol
            {
              //  Structure sp = _ctx.StructureSet.Structures.Where(_ctx.StructureSet.Structures.Id == s)


            }

            couchStructExist.setToINFO();
            couchStructExist.MeasuredValue = "pas encore de test (en cours)";// "Différent de Planning Approved";

            couchStructExist.Infobulle = "en cours";
            this._result.Add(couchStructExist);
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
