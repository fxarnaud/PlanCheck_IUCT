using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using System.Windows;
/*
 namespace PlanCheck_IUCT
{
    internal class Check_UM
    {
        private ScriptContext _ctx;

        public Check_UM(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {
            _ctx = ctx;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "UM";

        public void Check()
        {
           
*/
namespace PlanCheck_IUCT
{
    internal class Check_Isocenter
    {
        private ScriptContext _ctx;

        public Check_Isocenter(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {
            _ctx = ctx;
            Check();
        }



        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Isocentre";

        public void Check()
        {


            #region Origine vs Iso
            Item_Result origin = new Item_Result();
            origin.Label = "Isocentre et origine";
            origin.ExpectedValue = "sans objet";



            // var originwasChangedTitle = "User Origin Check";
            //var originwasChangedResult = "OK";
            //   var originwasChangedComment = "";
            var image = _ctx.PlanSetup.StructureSet.Image;
            if (!image.HasUserOrigin)
            {
                origin.setToWARNING();
                //originwasChangedResult = "Warning";
                //originwasChangedComment += "Origine = Dicom origine";
                origin.MeasuredValue = "origine = isocentre";
                origin.Infobulle = "L'origine et l'isocentre sont confondus. Ce qui peut signifier que l'origine n'a pas été placée. A vérifier.";
            }
            else
            {
                origin.setToTRUE();
                //originwasChangedResult = "OK";
                //originwasChangedComment += "origine différente de l'origine DICOM";
                origin.MeasuredValue = "Origine différente de l'isocentre";
                origin.Infobulle = "L'origine et l'isocentre ne sont pas confondus. Dans le cas contraire cela peut signifier que l'origine n'a pas été placée";
            }

            //var checkScreenOrigin = new CheckScreen(originwasChangedTitle, originwasChangedResult, originwasChangedComment);




            
            

            this._result.Add(origin);
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
