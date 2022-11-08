﻿using System;
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



            #region Tous les champs ont le même iso
            Item_Result allFieldsSameIso = new Item_Result();
            int numberOfIso = 0;
            double myx = 999999.0;
            double myy = 999999.0;
            double myz = 999999.0;
            allFieldsSameIso.Label = "Unicité de l'isocentre";
            allFieldsSameIso.ExpectedValue = "1";

            foreach (Beam b in _ctx.PlanSetup.Beams)
            {
                if ((myx != b.IsocenterPosition.x) || (myy != b.IsocenterPosition.y) || (myz != b.IsocenterPosition.z))
                {
                    myx = b.IsocenterPosition.x;
                    myy = b.IsocenterPosition.y;
                    myz = b.IsocenterPosition.z;
                    numberOfIso++;
                }
            }


            if (numberOfIso > 1)
            {
                allFieldsSameIso.setToFALSE();
                allFieldsSameIso.MeasuredValue = "Plusieurs isocentres";
            }
            else
            {
                allFieldsSameIso.setToTRUE();
                allFieldsSameIso.MeasuredValue = "Un seul isocentre";
            }


            allFieldsSameIso.Infobulle = "Tous les champs du plan doivent avoir le même isocentre, sauf plan multi-isocentres";


            this._result.Add(allFieldsSameIso);
            #endregion




            #region Iso au centre du PTV
            Item_Result isoAtCenterOfPTV = new Item_Result();

            isoAtCenterOfPTV.Label = "Poisition de l'isocentre"; 
            isoAtCenterOfPTV.ExpectedValue = "1";

            // position isocentre
            myx = myx + 0;
            myy = myy + 0;
            myz = myz + 0;

            Structure largestPTV = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id.ToLower().Contains("ptv"));
                       

            isoAtCenterOfPTV.setToFALSE();
            isoAtCenterOfPTV.MeasuredValue = "Plusieurs isocentres";


            isoAtCenterOfPTV.Infobulle = "Tous les champs du plan doivent avoir le même isocentre, sauf plan multi-isocentres";


            this._result.Add(allFieldsSameIso);
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
