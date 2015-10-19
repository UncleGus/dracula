using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DraculaSimulator
{
    [DataContract]
    public class Item
    {
        [DataMember]
        public string name { get; set; }
        
        public Item(string newName)
        {
            name = newName;
        }
    }
}
