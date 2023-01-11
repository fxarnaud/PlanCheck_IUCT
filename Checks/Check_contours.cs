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
        public static int _GetSlice(double z, StructureSet SS)
        {
            var imageRes = SS.Image.ZRes;
            return Convert.ToInt32((z - SS.Image.Origin.z) / imageRes);
        }

        public static int getNumberOfMissingSlices(Structure S, StructureSet SS)
        {
            
            var mesh = S.MeshGeometry.Bounds;
            int meshLow = _GetSlice(mesh.Z, SS);
            int meshUp = _GetSlice(mesh.Z + mesh.SizeZ, SS);


            int nHoles = 0;
            for (int i = meshLow; i <= meshUp; i++)
            {
                VMS.TPS.Common.Model.Types.VVector[][] vvv = S.GetContoursOnImagePlane(i);

                if (vvv.Length == 0)
                    nHoles++;

            }
            return nHoles;
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

            foreach (expectedStructure es in _rcp.myCouchExpectedStructures) // foreach couch element in the xls protocol file
            {
                double mydouble = 0;
                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == es.Name); // find a structure in ss with the same name
                if (struct1 == null) // if structure doesnt exist in ss
                    missingCouchStructures.Add(es.Name);
                else if (struct1.IsEmpty) // else if it exists but empty --> same
                    missingCouchStructures.Add(es.Name);
                else
                {
                    struct1.GetAssignedHU(out mydouble);
                    if (mydouble != es.HU)
                        wrongHUCouchStructures.Add(es.Name);
                }
            }


            if ((wrongHUCouchStructures.Count == 0) && (missingCouchStructures.Count == 0))
            {
                couchStructExist.setToTRUE();
                couchStructExist.MeasuredValue = "Présentes et UH corectes " + _rcp.myCouchExpectedStructures.Count.ToString() + "/" + _rcp.myCouchExpectedStructures.Count.ToString();
                couchStructExist.Infobulle = "Structures de tables attendues pour le protocole " + _rcp.protocolName + " :\n";
                foreach (expectedStructure es in _rcp.myCouchExpectedStructures) // foreach couch element in the xls protocol file
                {
                    couchStructExist.Infobulle += " - " + es.Name + "\n";
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

            foreach (expectedStructure es in _rcp.myClinicalExpectedStructures) // foreach clinical struct in the xls check-protocol file
            {
                //MessageBox.Show("here is " + es.Name);
                double mydouble = 0;
                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == es.Name); // find a structure in ss with the same name
                if (struct1 == null) // if structure doesnt exist in ss
                    missingClinicalStructures.Add(es.Name);
                else if (struct1.IsEmpty) // else if it exists but empty --> same
                    missingClinicalStructures.Add(es.Name);
                else
                {
                    if (es.HU != 9999) // 9999 if no assigned HU 
                    {
                        struct1.GetAssignedHU(out mydouble);
                        if (mydouble != es.HU)
                            wrongHUClinicalStructures.Add(es.Name);
                    }
                }
            }

            if ((wrongHUClinicalStructures.Count == 0) && (missingClinicalStructures.Count == 0))
            {
                clinicalStructuresItem.setToTRUE();
                clinicalStructuresItem.MeasuredValue = "Présentes et UH corectes " + _rcp.myClinicalExpectedStructures.Count.ToString() + "/" + _rcp.myClinicalExpectedStructures.Count.ToString();
                clinicalStructuresItem.Infobulle = "Structures de tables attendues pour le protocole " + _rcp.protocolName + " :\n";
                foreach (expectedStructure es in _rcp.myClinicalExpectedStructures)
                {
                    clinicalStructuresItem.Infobulle += " - " + es.Name + "\n";
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
            foreach (expectedStructure es in _rcp.myOptExpectedStructures)
            {
                double mydouble = 0;
                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == es.Name); // find a structure in ss with the same name
                if (struct1 == null) // if structure doesnt exist in ss
                    missingOptStructures.Add(es.Name);
                else if (struct1.IsEmpty) // else if it exists but empty --> same
                    missingOptStructures.Add(es.Name);
                else
                {
                    if (es.HU != 9999) // 9999 if no assigned HU 
                    {
                        struct1.GetAssignedHU(out mydouble);
                        if (mydouble != es.HU)
                            wrongHUOptStructures.Add(es.Name);
                    }
                    //MessageBox.Show("YES we found in ss " + el.Item1 + " " + struct1.Id + " " + mydouble.ToString());
                }
            }



            if ((wrongHUOptStructures.Count == 0) && (missingOptStructures.Count == 0))
            {
                optStructuresItem.setToTRUE();
                optStructuresItem.MeasuredValue = "Présentes et UH corectes " + _rcp.myOptExpectedStructures.Count.ToString() + "/" + _rcp.myOptExpectedStructures.Count.ToString();
                optStructuresItem.Infobulle = "Structures de tables attendues pour le protocole " + _rcp.protocolName + " :\n";
                foreach (expectedStructure es in _rcp.myOptExpectedStructures)
                {
                    optStructuresItem.Infobulle += " - " + es.Name + "\n";
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

            #region Concatenate all structures in one list
            var allStructures = _rcp.myClinicalExpectedStructures.Concat(_rcp.myOptExpectedStructures).Concat(_rcp.myCouchExpectedStructures).ToList();
            #endregion

            #region  Anormal Volume values (cc)
            // entre -3sigma et +3sigma >99.9% des cas
            List<string> anormalVolumeList = new List<string>();
            List<string> normalVolumeList = new List<string>();
            Item_Result anormalVolumeItem = new Item_Result();
            anormalVolumeItem.Label = "Volume des structures";
            anormalVolumeItem.ExpectedValue = "EN COURS";

            //foreach (expectedStructure es in _rcp.myClinicalExpectedStructures)
            foreach (expectedStructure es in allStructures)
            {

                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == es.Name); // find a structure in ss with the same name
                if (struct1 != null) // if structure  exist 
                    if (!struct1.IsEmpty) //  and if not empty 
                        if (es.volMin != 9999) // and if a volume min is defined in protocol
                        {
                            if ((struct1.Volume > es.volMin) && (struct1.Volume < es.volMax)) //if volume ok
                                normalVolumeList.Add(es.Name);
                            else
                                anormalVolumeList.Add(es.Name + " (" + struct1.Volume.ToString("F2") + " cc). Attendu: " + es.volMin.ToString("F2") + " - " + es.volMax.ToString("F2") + " cc");

                        }


            }
            if (anormalVolumeList.Count > 0)
            {
                anormalVolumeItem.setToWARNING();
                anormalVolumeItem.MeasuredValue = "Volumes anormaux détectés";
                anormalVolumeItem.Infobulle = "Les volumes des structures suivantes ne sont\npas dans l'intervalle 6 sigma des volumes habituels\n";
                foreach (string avs in anormalVolumeList)
                    anormalVolumeItem.Infobulle += " - " + avs + "\n";


            }
            else if (normalVolumeList.Count > 0)
            {
                anormalVolumeItem.setToTRUE();
                anormalVolumeItem.MeasuredValue = "Volumes des structures OK";
                anormalVolumeItem.Infobulle = "Les volumes des structures suivantes sont\ndans l'intervalle 6 sigma des volumes habituels\n";
                foreach (string avs in normalVolumeList)
                    anormalVolumeItem.Infobulle += " - " + avs + "\n";


            }
            else
            {
                anormalVolumeItem.setToINFO();
                anormalVolumeItem.MeasuredValue = "Aucune analyse de volumes de structures";
                anormalVolumeItem.Infobulle = "Les structures présentes n'ont pas de valeur de volumes (cc) attendus dans le check-protocol\n";
            }

            this._result.Add(anormalVolumeItem);


            #endregion

            #region Shape analyser (number of parts of a structure)
            /* Check if a structrure has the expected number of parts e.g. if a slice is missing */
            Item_Result shapeAnalyser = new Item_Result();
            shapeAnalyser.Label = "Nombre de parties des structures";
            shapeAnalyser.ExpectedValue = "wip...";


            List<string> correctStructs = new List<string>();
            List<string> uncorrectStructs = new List<string>();





            foreach (expectedStructure es in allStructures)
            {

                Structure struct1 = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == es.Name); // find a structure in ss with the same name


                if (struct1 != null)
                    if (!struct1.IsEmpty)
                        if (es.expectedNumberOfPart != 9999) // expected number of parts exists
                        {
                            int n = struct1.GetNumberOfSeparateParts();


                            if (n != es.expectedNumberOfPart)
                            {

                                uncorrectStructs.Add(es.Name + " comporte " + n + " parties (attendu : " + es.expectedNumberOfPart + ")");

                            }
                            else
                            {

                                correctStructs.Add(es.Name + " comporte " + n + " parties (attendu : " + es.expectedNumberOfPart + ")");
                            }
                        }

            }
            if (uncorrectStructs.Count > 0)
            {
                shapeAnalyser.setToFALSE();
                shapeAnalyser.MeasuredValue = " Nombres de parties des structures incorrects";
                shapeAnalyser.Infobulle = "Les structures suivantes ont un de nombre de parties non-conforme au check-protocol :\n";
                foreach (string s in uncorrectStructs)
                    shapeAnalyser.Infobulle += s + "\n";
            }
            else if (correctStructs.Count > 0)
            {
                shapeAnalyser.setToTRUE();

                shapeAnalyser.MeasuredValue = " Nombres de parties des structures corrects";
                shapeAnalyser.Infobulle = "Les structures suivantes ont un de nombre de parties conforme au check-protocol :\n";
                foreach (string s in correctStructs)
                    shapeAnalyser.Infobulle += s + "\n";

            }
            else
            {
                shapeAnalyser.setToINFO();
                shapeAnalyser.MeasuredValue = " Aucune analyse du nombres de parties des structures";
                shapeAnalyser.Infobulle = "Les structures présentes n'ont pas de valeurs attendues de nombre de parties dans le check-protocol\n";
            }



            this._result.Add(shapeAnalyser);
            #endregion

            #region missing slices
            Item_Result missingSlicesItem = new Item_Result();
            missingSlicesItem.Label = "Contours manquants";
            missingSlicesItem.ExpectedValue = "wip...";

            int m = 0;
            int nAnalysedStructures = 0;
            List<string> structureswithAGap = new List<string>();
            foreach (Structure s in _ctx.StructureSet.Structures)
            {
                if ((s.Id != "Plombs") && (!s.IsEmpty)) // do no check marker structures
                {
                    nAnalysedStructures++;
                    m = getNumberOfMissingSlices(s, _ctx.StructureSet);
                    if (m > 0)
                        structureswithAGap.Add(m.ToString() + "Contours manquantes pour: " + s.Id);
                }
            }
            if (structureswithAGap.Count > 0)
            {
                missingSlicesItem.MeasuredValue = "Certaines structures présentent des contours manquants";
                missingSlicesItem.setToWARNING();
                foreach (string s in structureswithAGap)
                    missingSlicesItem.Infobulle += s + "\n";

            }
            else
            {
                missingSlicesItem.MeasuredValue = "Aucune coupe non-contourée détectée";
                missingSlicesItem.setToTRUE();
                missingSlicesItem.Infobulle = nAnalysedStructures.ToString() + " structures analysées. Aucune coupe non-contourée détectée";
            }
            this._result.Add(missingSlicesItem);
            #endregion


            #region Laterality
            Item_Result laterality = new Item_Result();
            laterality.Label = "Lateralité";
            laterality.ExpectedValue = "wip...";

            List<string> goodLaterality = new List<string>();
            List<string> badLaterality = new List<string>();
            Structure sbody = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == "BODY"); // find body

            if (sbody == null) MessageBox.Show("BODY NOT FOUND");

            double bodyXcenter = sbody.MeshGeometry.Bounds.X + (sbody.MeshGeometry.Bounds.SizeX / 2.0);

            foreach (expectedStructure es in allStructures)
            {
                if (es.laterality != "NONE")
                {
                    Structure s = _ctx.StructureSet.Structures.FirstOrDefault(x => x.Id == es.Name); // find a structure in ss with the same name
                    double xpos = 0.0;
                    if (s != null)
                        if (!s.IsEmpty)
                        {
                            xpos = s.MeshGeometry.Bounds.X + (s.MeshGeometry.Bounds.SizeX / 2.0);  // (Left limit + size) /2

                            //MessageBox.Show("orientation : " + _ctx.Image.ImagingOrientation.ToString());
                            //if(_ctx.Image.ImagingOrientation) //

                            if (xpos > bodyXcenter) // THIS IS LEFT,  if Supine HF but also Prone HF, Supine FF...
                            {
                                if (es.laterality == "L")
                                    goodLaterality.Add(es.Name);
                                else if (es.laterality == "R")
                                    badLaterality.Add(es.Name);
                            }
                            else
                            {
                                if (es.laterality == "R")
                                    goodLaterality.Add(es.Name);
                                else if (es.laterality == "L")
                                    badLaterality.Add(es.Name);

                            }

    //                            MessageBox.Show(s.Id + " " + s.MeshGeometry.Bounds.X.ToString("0.00") + " " + s.MeshGeometry.Bounds.SizeX.ToString("0.00") +" "+  xpos.ToString("0.00") + " " + bodyXcenter.ToString("0.00")); //+ (0.5 - tolerance) * (ptvTarget.MeshGeometry.Bounds.SizeX);

                        }  
                }
            }

            if (badLaterality.Count > 0)
            {
                laterality.MeasuredValue = "Mauvaise latéralité (voir détail)";
                laterality.setToFALSE();

                laterality.Infobulle = "Ces structures sont attendues à gauche ou à droite et semblent du mauvais côté : \n";
                foreach (string s in badLaterality)
                    laterality.Infobulle += " - " + s + "\n";
            }
            else
            {
                laterality.MeasuredValue = "Vérifiée pour " + goodLaterality.Count() + " structures";
                laterality.setToTRUE();

                laterality.Infobulle = "Ces structures sont attendues à gauche ou à droite et semblent du bon côté : \n";
                foreach (string s in goodLaterality)
                    laterality.Infobulle += " - " + s + "\n";

            }


            this._result.Add(laterality);
            #endregion

            #region A PTV for EACH CTV/GTV
            Item_Result aPTVforEvryone = new Item_Result();
            aPTVforEvryone.Label = "GTV PTV";
            aPTVforEvryone.ExpectedValue = "wip...";


            aPTVforEvryone.MeasuredValue = ". . . . ";
            aPTVforEvryone.setToTRUE();
            aPTVforEvryone.Infobulle = " - - - - -";

            this._result.Add(aPTVforEvryone);
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
