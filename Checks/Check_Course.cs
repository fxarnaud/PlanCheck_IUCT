using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck_IUCT
{
    internal class Check_Course
    {
        private ScriptContext _ctx;
        private int maxNumberOfDays = 5;
        public Check_Course(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {     
            _ctx = ctx;
            Check();
            
        }

        private List<Item_Result> _result = new List<Item_Result>();
       // private PreliminaryInformation _pinfo;
        private string _title = "Statut des Courses et du plan";

        public void Check()
        {

            #region Plan approuvé ? 
            Item_Result approve = new Item_Result();
            approve.Label = "Statut d'approbation du plan";
            approve.ExpectedValue = "EN COURS";

            if (_ctx.PlanSetup.ApprovalStatus.ToString() == "PlanningApproved")
            {
                approve.MeasuredValue = "Plan approuvé ";// + _ctx.PlanSetup.PlanningApprover;
                approve.setToTRUE();
            }
            else
            {
                approve.MeasuredValue = _ctx.PlanSetup.ApprovalStatus.ToString();// "Différent de Planning Approved";
                approve.setToFALSE();
            }
            approve.Infobulle = "Le plan doit être Planning Approved";
            this._result.Add(approve);
            #endregion


            #region ACTUAL COURSE
            Item_Result currentCourseStatus = new Item_Result();
            currentCourseStatus.Label =  "Course actuel : " + _ctx.Course.Id;
            currentCourseStatus.ExpectedValue = "EN COURS";
            if(_ctx.Course.CompletedDateTime == null)
            {
                currentCourseStatus.MeasuredValue = "EN COURS";
                currentCourseStatus.setToTRUE();

            }
            else
            {
                currentCourseStatus.MeasuredValue = "TERMINE";
                currentCourseStatus.setToFALSE();
            }
            currentCourseStatus.Infobulle = "L'état du course actuel doit être EN COURS";
            this._result.Add(currentCourseStatus);
            #endregion

            #region other courses
            foreach (Course courseN in _ctx.Patient.Courses) // loop on the courses
            {
                Item_Result myCourseStatus = new Item_Result();
                myCourseStatus.ExpectedValue = "TERMINE";
                //myCourseStatus.Comparator = "=";
                myCourseStatus.Infobulle = "OK si TERMINE, WARNING si en cours depuis < "+ maxNumberOfDays +" jours, X sinon";
                //Comparator testing = new Comparator();
                myCourseStatus.Label = "Course : " + courseN.Id;

                if (courseN.Id != _ctx.Course.Id) // do not test current course
                    if (courseN.CompletedDateTime != null) // --> terminated courses = there is a  completed date time
                    {
                        myCourseStatus.MeasuredValue = "TERMINE";
                        myCourseStatus.setToTRUE();
                        //myCourseStatus.ResultStatus = testing.CompareDatas(myCourseStatus.ExpectedValue, myCourseStatus.MeasuredValue, myCourseStatus.Comparator);
                        this._result.Add(myCourseStatus);
                    }
                    else // course en cours
                    {
                        DateTime myToday = DateTime.Today;
                        int nDays = (myToday - (DateTime)courseN.HistoryDateTime).Days;
                        if (nDays < maxNumberOfDays)
                            myCourseStatus.setToWARNING();
                        else
                            myCourseStatus.setToFALSE();

                        myCourseStatus.MeasuredValue = "EN COURS depuis " + nDays.ToString() + " jours";

                        //myCourseStatus.ResultStatus = testing.CompareDatas(myCourseStatus.ExpectedValue, myCourseStatus.MeasuredValue, myCourseStatus.Comparator);
                        this._result.Add(myCourseStatus);

                    }
            }
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
