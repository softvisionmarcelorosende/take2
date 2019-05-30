using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Take2.Source.Models
{
    public class KeyValue
    {
        public string Etag { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public string Content_Type { get; set; }
        public string Value { get; set; }
        public object Tags { get; set; }
        public string Locked { get; set; }
        public string Last_Modified { get; set; }

    }


}
