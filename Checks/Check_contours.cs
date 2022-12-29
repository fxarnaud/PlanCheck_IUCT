using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using System.Windows;

namespace PlanCheck_IUCT
{
    internal class Check_contours
    {
        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;
        private read_check_protocol _rcp;

        public Check_contours(PreliminaryInformation pinfo, ScriptContext ctx, read_check_protocol rcp)  //Constructor
        {
            _rcp = rcp;
            _ctx = ctx;
            _pinfo = pinfo;
            Check();

        }

        private List<Item_Result> _result = new List<Item_Result>();
        // private PreliminaryInformation _pinfo;
        private string _title = "Contours";

        public void Check()
        {
            
            #region COUCH STRUCTURES 
            Item_Result couchStructExist = new Item_Result();
            couchStructExist.Label = "Structures de table";
            couchStructExist.ExpectedValue = "EN COURS";

            List<string> missingCouchStructures = new List<string>();
            List<string> wrongHUCouchStructures = new List<string>();
            foreach (Tuple<string, double> el in _rcp.couchStructures) // foreach couch element in the xls protocol file
            {
                double mydouble = 0;
                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == el.Item1); // find a structure in ss with the same name
                if (struct1 == null) // if structure doesnt exist in ss
                    missingCouchStructures.Add(el.Item1);
                else if (struct1.IsEmpty) // else if it exists but empty --> same
                    missingCouchStructures.Add(el.Item1);
                else
                {
                    struct1.GetAssignedHU(out mydouble);
                    if (mydouble != el.Item2)
                        wrongHUCouchStructures.Add(el.Item1);
                    //MessageBox.Show("YES we found in ss " + el.Item1 + " " + struct1.Id + " " + mydouble.ToString());
                }
            }



            if ((wrongHUCouchStructures.Count == 0) && (missingCouchStructures.Count == 0))
            {
                couchStructExist.setToTRUE();
                couchStructExist.MeasuredValue = "Présentes et UH corectes " + _rcp.couchStructures.Count.ToString() + "/" + _rcp.couchStructures.Count.ToString();
                couchStructExist.Infobulle = "Structures de tables attendues pour le protocole " + _rcp.protocolName + " :\n";
                foreach (Tuple<string, double> el in _rcp.couchStructures) // foreach couch element in the xls protocol file
                {
                    couchStructExist.Infobulle += " - " + el.Item1 + "\n";
                }
            }
            else
            {
                couchStructExist.setToFALSE();
                couchStructExist.MeasuredValue = "Absentes, vides ou UH incorrectes (voir infobulle)";
                if (missingCouchStructures.Count > 0)
                    couchStructExist.Infobulle = "Structures attendues pour le protocole " + _rcp.protocolName + " absentes ou vides dans le plan :\n";
                foreach (string ms in missingCouchStructures)
                    couchStructExist.Infobulle += " - " + ms + "\n";
                if (wrongHUCouchStructures.Count > 0)
                    couchStructExist.Infobulle += "Structures avec UH incorrectes :\n";
                foreach (string ms in wrongHUCouchStructures)
                    couchStructExist.Infobulle += " - " + ms + "\n";

            }


            this._result.Add(couchStructExist);
            #endregion


            #region CLINICAL STRUCTURES 

            Item_Result clinicalStructuresItem = new Item_Result();
            clinicalStructuresItem.Label = "Structures cliniques";
            clinicalStructuresItem.ExpectedValue = "EN COURS";


            List<string> missingClinicalStructures = new List<string>();
            List<string> wrongHUClinicalStructures = new List<string>();
            foreach (Tuple<string, double,double,double> el in _rcp.clinicalStructures) // foreach couch element in the xls protocol file
            {
                double mydouble = 0;
                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == el.Item1); // find a structure in ss with the same name
                if (struct1 == null) // if structure doesnt exist in ss
                    missingClinicalStructures.Add(el.Item1);
                else if (struct1.IsEmpty) // else if it exists but empty --> same
                    missingClinicalStructures.Add(el.Item1);
                else
                {
                    if (el.Item2 != 9999) // 9999 if no assigned HU 
                    {
                        struct1.GetAssignedHU(out mydouble);
                        if (mydouble != el.Item2)
                            wrongHUClinicalStructures.Add(el.Item1);
                    }
                    //MessageBox.Show("YES we found in ss " + el.Item1 + " " + struct1.Id + " " + mydouble.ToString());
                }
            }



            if ((wrongHUClinicalStructures.Count == 0) && (missingClinicalStructures.Count == 0))
            {
                clinicalStructuresItem.setToTRUE();
                clinicalStructuresItem.MeasuredValue = "Présentes et UH corectes " + _rcp.clinicalStructures.Count.ToString() + "/" + _rcp.clinicalStructures.Count.ToString();
                clinicalStructuresItem.Infobulle = "Structures de tables attendues pour le protocole " + _rcp.protocolName + " :\n";
                foreach (Tuple<string, double,double,double> el in _rcp.clinicalStructures) // foreach couch element in the xls protocol file
                {
                    clinicalStructuresItem.Infobulle += " - " + el.Item1 + "\n";
                }
            }
            else
            {
                clinicalStructuresItem.setToINFO(); // just info except if wrong HU --> warrning
                if (wrongHUClinicalStructures.Count > 0)
                    clinicalStructuresItem.setToWARNING();

                clinicalStructuresItem.MeasuredValue = "Absentes, vides ou UH incorrectes (voir infobulle)";
                if (missingClinicalStructures.Count > 0)
                    clinicalStructuresItem.Infobulle = "Structures attendues pour le protocole " + _rcp.protocolName + " absentes ou vides dans le plan :\n";
                foreach (string ms in missingClinicalStructures)
                    clinicalStructuresItem.Infobulle += " - " + ms + "\n";
                if (wrongHUClinicalStructures.Count > 0)
                    clinicalStructuresItem.Infobulle += "Structures avec UH incorrectes :\n";
                foreach (string ms in wrongHUClinicalStructures)
                    clinicalStructuresItem.Infobulle += " - " + ms + "\n";

            }


            this._result.Add(clinicalStructuresItem);
            #endregion



            #region OPT STRUCTURES 

            Item_Result optStructuresItem = new Item_Result();
            optStructuresItem.Label = "Structures d'optimisation";
            optStructuresItem.ExpectedValue = "EN COURS";


            List<string> missingOptStructures = new List<string>();
            List<string> wrongHUOptStructures = new List<string>();
            foreach (Tuple<string, double> el in _rcp.optStructures) // foreach couch element in the xls protocol file
            {
                double mydouble = 0;
                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == el.Item1); // find a structure in ss with the same name
                if (struct1 == null) // if structure doesnt exist in ss
                    missingOptStructures.Add(el.Item1);
                else if (struct1.IsEmpty) // else if it exists but empty --> same
                    missingOptStructures.Add(el.Item1);
                else
                {
                    if (el.Item2 != 9999) // 9999 if no assigned HU 
                    {
                        struct1.GetAssignedHU(out mydouble);
                        if (mydouble != el.Item2)
                            wrongHUOptStructures.Add(el.Item1);
                    }
                    //MessageBox.Show("YES we found in ss " + el.Item1 + " " + struct1.Id + " " + mydouble.ToString());
                }
            }



            if ((wrongHUOptStructures.Count == 0) && (missingOptStructures.Count == 0))
            {
                optStructuresItem.setToTRUE();
                optStructuresItem.MeasuredValue = "Présentes et UH corectes " + _rcp.optStructures.Count.ToString() + "/" + _rcp.optStructures.Count.ToString();
                optStructuresItem.Infobulle = "Structures de tables attendues pour le protocole " + _rcp.protocolName + " :\n";
                foreach (Tuple<string, double> el in _rcp.optStructures) // foreach couch element in the xls protocol file
                {
                    optStructuresItem.Infobulle += " - " + el.Item1 + "\n";
                }
            }
            else
            {
                optStructuresItem.setToINFO();
                optStructuresItem.MeasuredValue = "Absentes, vides ou UH incorrectes (voir infobulle)";
                if (missingOptStructures.Count > 0)
                    optStructuresItem.Infobulle = "Structures attendues pour le protocole " + _rcp.protocolName + " absentes ou vides dans le plan :\n";
                foreach (string ms in missingOptStructures)
                    optStructuresItem.Infobulle += " - " + ms + "\n";
                if (wrongHUOptStructures.Count > 0)
                    optStructuresItem.Infobulle += "Structures avec UH incorrectes :\n";
                foreach (string ms in wrongHUOptStructures)
                    optStructuresItem.Infobulle += " - " + ms + "\n";

            }


            this._result.Add(optStructuresItem);
            #endregion

            
            #region Volume Anormal
            // entre -3sigma et +3sigma >99.9% des cas
            List<string> anormalVolumeList = new List<string>();
            List<string> normalVolumeList = new List<string>();
            Item_Result anormalVolumeItem = new Item_Result();
            anormalVolumeItem.Label = "Volume des structures";
            anormalVolumeItem.ExpectedValue = "EN COURS";

            foreach (Tuple<string, double, double, double> el in _rcp.clinicalStructures) // foreach couch element in the xls protocol file
            {
               
                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == el.Item1); // find a structure in ss with the same name
                if (struct1 != null) // if structure  exist 
                    if (!struct1.IsEmpty) //  and if not empty 
                        if (el.Item3 != 9999) // and if a volume min is defined in protocol
                        {
                            if ((struct1.Volume > el.Item3) && (struct1.Volume < el.Item4)) //if volume ok
                                normalVolumeList.Add(el.Item1);
                            else
                                anormalVolumeList.Add(el.Item1 + " ("+struct1.Volume.ToString("F2")+" cc). Attendu: "+ el.Item3.ToString("F2")+" - "+el.Item4.ToString("F2") + " cc");

                        }


            }
            if (anormalVolumeList.Count > 0)
            {
                anormalVolumeItem.setToWARNING();
                anormalVolumeItem.MeasuredValue = "Volumes anormaux détectés";
                anormalVolumeItem.Infobulle = "Les volumes des structures suivantes ne sont\npas dans l'intervalle 6 sigma des volumes habituels\n";
                foreach(string avs in anormalVolumeList)
                    anormalVolumeItem.Infobulle += " - " + avs + "\n";

                this._result.Add(anormalVolumeItem);
            }
            else if (normalVolumeList.Count > 0)
            {
                anormalVolumeItem.setToTRUE();
                anormalVolumeItem.MeasuredValue = "Volumes des structures OK";
                anormalVolumeItem.Infobulle = "Les volumes des structures suivantes sont\ndans l'intervalle 6 sigma des volumes habituels\n";
                foreach (string avs in normalVolumeList)
                    anormalVolumeItem.Infobulle += " - " + avs + "\n";

                this._result.Add(anormalVolumeItem);
            }

          

//          this._result.Add(anormalVolumeItem);

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
