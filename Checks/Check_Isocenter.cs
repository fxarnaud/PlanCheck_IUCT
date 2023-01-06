﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using System.Windows;

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

            isoAtCenterOfPTV.Label = "Position de l'isocentre";
            isoAtCenterOfPTV.ExpectedValue = "1";
            isoAtCenterOfPTV.setToTRUE();

            Structure ptvTarget=null;// = new Structure;
           

            foreach (Structure s in _ctx.StructureSet.Structures)
            {
                if (s.Id == _ctx.PlanSetup.TargetVolumeID) 
                {
                    ptvTarget = s;
                }
            }
            

            // looking if isocenter is close to the ptv center
            // Coordinates are in DICOM ref 

            double tolerance = 0.15; // 0.1 means that we expect the isocenter in a region  from + or -10% around the center of PTV

            double centerPTVxmin = ptvTarget.MeshGeometry.Bounds.X + (0.5 - tolerance) * (ptvTarget.MeshGeometry.Bounds.SizeX);
            double centerPTVymin = ptvTarget.MeshGeometry.Bounds.Y + (0.5 - tolerance) * (ptvTarget.MeshGeometry.Bounds.SizeY);
            double centerPTVzmin = ptvTarget.MeshGeometry.Bounds.Z + (0.5 - tolerance) * (ptvTarget.MeshGeometry.Bounds.SizeZ);

            double centerPTVxmax = ptvTarget.MeshGeometry.Bounds.X + (0.5 + tolerance) * (ptvTarget.MeshGeometry.Bounds.SizeX);
            double centerPTVymax = ptvTarget.MeshGeometry.Bounds.Y + (0.5 + tolerance) * (ptvTarget.MeshGeometry.Bounds.SizeY);
            double centerPTVzmax = ptvTarget.MeshGeometry.Bounds.Z + (0.5 + tolerance) * (ptvTarget.MeshGeometry.Bounds.SizeZ);

            double fractionX = (myx - ptvTarget.MeshGeometry.Bounds.X)/ ptvTarget.MeshGeometry.Bounds.SizeX;
            double fractionY = (myy - ptvTarget.MeshGeometry.Bounds.Y) / ptvTarget.MeshGeometry.Bounds.SizeY;
            double fractionZ = (myz - ptvTarget.MeshGeometry.Bounds.Z) / ptvTarget.MeshGeometry.Bounds.SizeZ;



            int iswrong = 0;
            if ((myx > centerPTVxmax) || (myx < centerPTVxmin))
            {               
                iswrong = 1;                
            }
            if ((myy > centerPTVymax) || (myy < centerPTVymin))
            {                
                iswrong = 1;                
            }
            if ((myz > centerPTVzmax) || (myz < centerPTVzmin))
            {                
                iswrong = 1;                
            }
            if (iswrong == 1)
            {
                isoAtCenterOfPTV.MeasuredValue = " Positionnement non central de l'isocentre dans le " + ptvTarget.Id;
                isoAtCenterOfPTV.setToWARNING();
            }
            else
            {
                isoAtCenterOfPTV.MeasuredValue = " Isocentre proche du centre de " + ptvTarget.Id;
                isoAtCenterOfPTV.setToTRUE();
            }

            double tolmin = 0.5 - tolerance;
            double tolmax = 0.5 + tolerance;
            isoAtCenterOfPTV.Infobulle = "L'isocentre doit être proche du centre de " + ptvTarget.Id;
            isoAtCenterOfPTV.Infobulle += "\n(volume cible)";
            isoAtCenterOfPTV.Infobulle += "\navec une tolérance de " + (tolerance * 100).ToString("N1") + "% dans chaque direction.";
            isoAtCenterOfPTV.Infobulle += "\n\nPosition relative de l'isoscentre sur les axes x y et z:\n" + Math.Round(fractionX,2) + "\t" + Math.Round(fractionY, 2) + "\t" + Math.Round(fractionZ, 2);
            isoAtCenterOfPTV.Infobulle += "\n\n0 et 1 = limites du PTV";
            isoAtCenterOfPTV.Infobulle += "\nValeures attendues entre "+ tolmin + " et " + tolmax;

            this._result.Add(isoAtCenterOfPTV);
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
