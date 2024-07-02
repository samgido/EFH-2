using System.Xml.Serialization;

namespace EFH2
{
	public class RcnDataModel
    {
        [XmlElement("Group A")]
        public RcnGroupModel GroupA = new();
        [XmlElement("Group B")]
        public RcnGroupModel GroupB = new();
        [XmlElement("Group C")]
        public RcnGroupModel GroupC = new();
        [XmlElement("Group D")]
        public RcnGroupModel GroupD = new();

        // Doesn't make sense to store these 
        //public double AccumulatedArea => GroupA.AccumulatedArea + GroupB.AccumulatedArea + GroupC.AccumulatedArea + GroupD.AccumulatedArea;

        //public double AccumulatedWeightedArea => GroupA.AccumulatedWeightedArea + GroupB.AccumulatedWeightedArea + GroupC.AccumulatedWeightedArea + GroupD.AccumulatedWeightedArea;
    }
}
