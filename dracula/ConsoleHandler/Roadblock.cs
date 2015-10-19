using LocationHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DraculaSimulator
{
    [DataContract]
    public class Roadblock
    {
        [DataMember]
        public LocationDetail firstLocation { get; set; }
        [DataMember]
        public LocationDetail secondLocation { get; set; }
        [DataMember]
        public string connectionType { get; set; }
    }
}
