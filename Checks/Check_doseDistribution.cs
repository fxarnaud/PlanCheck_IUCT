using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows;
using System.Text.RegularExpressions;

namespace PlanCheck_IUCT
{
    internal class Check_doseDistribution
    {
        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;
        private read_check_protocol _rcp;
        public Check_doseDistribution(PreliminaryInformation pinfo, ScriptContext ctx, read_check_protocol rcp)  //Constructor
        {
            _ctx = ctx;
            _pinfo = pinfo;
            _rcp = rcp;
            Check();

        }

        public double getValueForThisObjective(DVHData dvh, string theObjective, string unit)
        {
            double result = 0.0;


            if (dvh != null)
                if (theObjective == "mean")
                {
                    var myMeanDose = dvh.MeanDose;
                    result = myMeanDose.Dose;

                }
            //if(theObjective)

            // pense quand s est null
            return result;
        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Dose Distribution (en cours)";

        public void Check()
        {

            #region Objectives to reach             
            Item_Result dd = new Item_Result();
            dd.Label = "Objectifs de dose";
            dd.ExpectedValue = "EN COURS";
            List<string> successList = new List<string>();
            List<string> failedList = new List<string>();
            dd.setToINFO();
            dd.MeasuredValue = "en cours";// "Différent de Planning Approved";
            //Regex regexInf = new Regex("[A-Za-z0-9]+<[0-9]*\\.[0-9]+[a-zA-Z]+", RegexOptions.IgnoreCase); // obj inferior
            //Regex regexSup = new Regex("[A-Za-z0-9]+>[0-9]*\\.[0-9]+[a-zA-Z]+", RegexOptions.IgnoreCase); // ob superior
            string infFormatPattern = "([A-Za-z0-9]+)(<)([0-9]*\\.[0-9]+)([a-zA-Z]+)"; // this waits for an expression a<xa  (a string,x double)
            string supFormatPattern = "([A-Za-z0-9]+)(>)([0-9]*\\.[0-9]+)([a-zA-Z]+)";


            foreach (DOstructure dos in _rcp.myDOStructures) // loop on list structures with objectivs in check-protocol
            {
                Structure s = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == dos.Name); // get the chosen structure
                DVHData dvh = null;

                if (s != null) // get the dvh once per struct
                {
                    dvh = _ctx.PlanSetup.GetDVHCumulativeData(s, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);
                }
                if (dvh != null)
                    foreach (string obj in dos.listOfObjectives) // loop on list of objectives in check-protocol
                    {

                        string message = "love";
                        string theObjective = "";
                        double theValue = 0.0;
                        string theUnit = "";
                        MatchCollection mi = Regex.Matches(obj, infFormatPattern);
                        MatchCollection ms = Regex.Matches(obj, supFormatPattern);
                        bool isInfObj = false;
                        bool isSupObj = false;
                        if (mi != null) // get an objective <
                        {
                            isInfObj = true;
                            if (mi.Count > 0)
                            {


                                int i = 0;
                                foreach (Group g in mi[0].Groups)  // well I dont like regex finally. It sucks.. but it works
                                {
                                    if (i == 1)
                                        theObjective = g.Value;
                                    if (i == 3)
                                        theValue = Convert.ToDouble(g.Value) + 1.0;
                                    if (i == 4)
                                        theUnit = g.Value;

                                    i++;
                                }
                            }
                        }
                        if (ms != null) // get an objective > 
                        {
                            isSupObj = true;
                            if (ms.Count > 0)
                            {
                                int i = 0;
                                foreach (Group g in ms[0].Groups)  // well I dont like regex finally. It sucks.. but it works
                                {
                                    if (i == 1)
                                        theObjective = g.Value;
                                    if (i == 3)
                                        theValue = Convert.ToDouble(g.Value) + 1.0;
                                    if (i == 4)
                                        theUnit = g.Value;

                                    i++;
                                }
                            }
                        }

                        double result = getValueForThisObjective(dvh, theObjective, theUnit);

                        if (isInfObj) // it is an inferior obj                           
                            if (result < theValue)
                                successList.Add(dos.Name + " " + obj + " Valeur du plan : " + result.ToString("0.00"));
                            else
                                failedList.Add(dos.Name + " " + obj + " Valeur du plan : " + result.ToString("0.00"));
                        else if (isSupObj)
                            if (result > theValue)
                                successList.Add(dos.Name + " " + obj + " Valeur du plan : " + result.ToString("0.00"));
                            else
                                failedList.Add(dos.Name + " " + obj + " Valeur du plan : " + result.ToString("0.00"));


                    }


            }
            dd.setToINFO();
            dd.Infobulle = "Aucun test réalisé";
            if ((successList.Count > 0) || (failedList.Count > 0))
            {
                if (failedList.Count > 0)
                {
                    dd.setToFALSE();
                    dd.MeasuredValue = "Au moins un objectif non atteint (voir détail)";
                }
                else
                {
                    dd.setToTRUE();
                    dd.MeasuredValue = "Tous les objectifs atteints";
                }
                dd.Infobulle = "";
                if (failedList.Count > 0)
                {
                    dd.Infobulle += "Echecs : \n";
                    foreach (string s in failedList)
                        dd.Infobulle += "  - " + s + "\n";
                }
                if (successList.Count > 0)
                {
                    dd.Infobulle += "Succès : \n";
                    foreach (string s in successList)
                        dd.Infobulle += "  - " + s + "\n";
                }
            }

            this._result.Add(dd);
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
