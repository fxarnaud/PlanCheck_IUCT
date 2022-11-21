using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using System.Windows;
using System.Windows.Navigation;
using System.Drawing;



namespace PlanCheck_IUCT
{
    internal class Check_Course
    {
        private ScriptContext _ctx;
        private PreliminaryInformation _pinfo;
        private int maxNumberOfDays = 8;
        public Check_Course(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {
            _ctx = ctx;
            _pinfo = pinfo;
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
            String[] beautifulDoctorName = _ctx.PlanSetup.PlanningApprover.Split('\\');
            String[] TAname = _ctx.PlanSetup.TreatmentApprover.Split('\\');
            approve.Infobulle = "Le plan doit être Planning Approved";
            if (_ctx.PlanSetup.ApprovalStatus.ToString() == "PlanningApproved")
            {

                approve.MeasuredValue = "Plan approuvé par le Dr " + beautifulDoctorName[1].ToUpper();// + _ctx.PlanSetup.PlanningApprover;s[0].ToString().ToUpper() + s.Substring(1);
                approve.setToTRUE();
                
            }
            else if (_ctx.PlanSetup.ApprovalStatus.ToString() == "TreatmentApproved")
            {

                approve.MeasuredValue = "Treatment approved";
                //Approuvé par le Dr " + _ctx.PlanSetup.TreatmentApprover + [1].ToUpper();// + _ctx.PlanSetup.PlanningApprover;s[0].ToString().ToUpper() + s.Substring(1);
                approve.Infobulle += "\n\nLe plan est en état Treat Approved";
                approve.Infobulle += "\nPlanning approver: " + beautifulDoctorName[1].ToUpper() + "\nTreatment approver " + TAname[1].ToUpper();
                approve.setToWARNING();
            }
            else
            {
                approve.MeasuredValue = _ctx.PlanSetup.ApprovalStatus.ToString();// "Différent de Planning Approved";
                approve.setToFALSE();
                //approve.Infobulle = "Le plan doit être Planning Approved";
            }

            this._result.Add(approve);
            #endregion


            #region ACTUAL COURSE
            Item_Result currentCourseStatus = new Item_Result();
            currentCourseStatus.Label = "Course " + _ctx.Course.Id +" (Course ouvert)";
            currentCourseStatus.ExpectedValue = "EN COURS";
            if (_ctx.Course.CompletedDateTime == null)
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
                myCourseStatus.Infobulle = "OK si TERMINE ou si course avec plan CQ en cours depuis < " + maxNumberOfDays + " jours";
                myCourseStatus.Infobulle += "\nWARNING si ne contient pas de plan CQ et en cours depuis < " + maxNumberOfDays;
                myCourseStatus.Infobulle += "\nErreur sinon (en cours depuis > "+ maxNumberOfDays + " jours)";
                //Comparator testing = new Comparator();
                myCourseStatus.Label = "Autre course : " + courseN.Id;

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
                        {
                            int itIsAQAPlan = 0;
                            foreach (PlanSetup p in courseN.PlanSetups)
                            {
                                if(p.PlanIntent.ToString() == "VERIFICATION") // is there a QA plan in the course ? 
                                {
                                    itIsAQAPlan = 1;
                                }
                            }
                            if (itIsAQAPlan == 0)
                            {
                                myCourseStatus.setToWARNING();
                                myCourseStatus.MeasuredValue = "Course sans plan CQ < " + nDays.ToString() + " jours";
                            }
                            else
                            {
                                myCourseStatus.setToTRUE();
                                myCourseStatus.MeasuredValue = "Contient un plan CQ < " + nDays.ToString() + " jours";
                            }
                        }
                        else
                        {
                            myCourseStatus.setToFALSE();
                            myCourseStatus.MeasuredValue = "EN COURS depuis " + nDays.ToString() + " jours (max "+maxNumberOfDays+" jours)";
                        }

                        

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
