using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlanCheck_IUCT
{
    public class Item_Result
    {
        private string _label;
        private string _expectedvalue;
        private string _measuredvalue;
        private string _comparator;
        private (string, SolidColorBrush) _resultstatus;
        private string _infobulle;

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }
        public string ExpectedValue
        {
            get { return _expectedvalue; }
            set { _expectedvalue = value; }
        }
        public string MeasuredValue
        {
            get { return _measuredvalue; }
            set { _measuredvalue = value; }
        }
        public string Comparator
        {
            get { return _comparator; }
            set { _comparator = value; }
        }
        public (string, SolidColorBrush) ResultStatus
        {
            get { return _resultstatus; }
            set { _resultstatus = value; }
        }
        public string Infobulle
        {
            get { return _infobulle; }
            set { _infobulle = value; }
        }
        public void setToTRUE()
        {
            this.ResultStatus = ("OK", new SolidColorBrush(Colors.LightGreen));
        }
        public void setToFALSE()
        {
            this.ResultStatus = ("X", new SolidColorBrush(Colors.LightSalmon));// .Red ?
        }
        public void setToINFO()
        {
            this.ResultStatus = ("INFO", new SolidColorBrush(Colors.LightGray));
        }
        public void setToWARNING()
        {
            this.ResultStatus = ("WARNING", new SolidColorBrush(Colors.LightYellow)); // .Light Salmon ? 
        }

       
    }
}
