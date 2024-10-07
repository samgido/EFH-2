using System.Collections.Generic;
using System.Xml.Serialization;

namespace EFH2
{
	public class RcnGroupModel
    {
        [XmlElement("Entries")]
        public List<RcnEntryModel> Entries = new List<RcnEntryModel>();
    }
}
