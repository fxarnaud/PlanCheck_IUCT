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

        public Check_Course(PreliminaryInformation pinfo, ScriptContext ctx)  //Constructor
        {     
            _ctx = ctx;
            Check();
            // hello
        }

        private List<Item_Result> _result = new List<Item_Result>();
        private PreliminaryInformation _pinfo;
        private string _title = "Course";

        public void Check()
        {

            foreach (Course courseN in _ctx.Patient.Courses) // loop on the courses
            {
                Item_Result myCourseStatus = new Item_Result();
                myCourseStatus.ExpectedValue = "TERMINE";
                myCourseStatus.Comparator = "=";
                myCourseStatus.Infobulle = "infobulle";
                Comparator testing = new Comparator();
                myCourseStatus.Label = courseN.Id;

                if (courseN.Id != _ctx.Course.Id) // do not test current course
                    if (courseN.CompletedDateTime != null) // --> terminated courses = there is a  completed date time
                    {
                        myCourseStatus.MeasuredValue = "TERMINE";
                        myCourseStatus.ResultStatus = testing.CompareDatas(myCourseStatus.ExpectedValue, myCourseStatus.MeasuredValue, myCourseStatus.Comparator);
                        this._result.Add(myCourseStatus);
                    }
                    else // course en cours
                    {
                        DateTime myToday = DateTime.Today;
                        int nDays = (myToday - (DateTime)courseN.HistoryDateTime).Days;
                        myCourseStatus.MeasuredValue = "EN COURS depuis " + nDays.ToString() + " jours";
                        myCourseStatus.ResultStatus = testing.CompareDatas(myCourseStatus.ExpectedValue, myCourseStatus.MeasuredValue, myCourseStatus.Comparator);
                        this._result.Add(myCourseStatus);

                    }
            }
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
