using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2.Models
{
    public class RcnDataModel
    {
        public RcnGroupModel GroupA;
        public RcnGroupModel GroupB;
        public RcnGroupModel GroupC;
        public RcnGroupModel GroupD;

        public double AccumulatedArea => GroupA.AccumulatedArea + GroupB.AccumulatedArea + GroupC.AccumulatedArea + GroupD.AccumulatedArea;

        public double AccumulatedWeightedArea => GroupA.AccumulatedWeightedArea + GroupB.AccumulatedWeightedArea + GroupC.AccumulatedWeightedArea + GroupD.AccumulatedWeightedArea;
    }
}
