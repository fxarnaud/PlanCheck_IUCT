using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanCheck_IUCT
{
    public class expectedStructure
    {
        public string Name { get; set; }
        public double HU { get; set; }
        public double volMin { get; set; }
        public double volMax { get; set; }
        public int expectedNumberOfPart { get; set; }
        public string laterality { get; set; }

    }
}
