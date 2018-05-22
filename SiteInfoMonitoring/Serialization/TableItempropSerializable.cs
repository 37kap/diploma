using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteInfoMonitoring.Serialization
{
    [Serializable]
    public class TableItempropSerializable : ItempropSerializable
    {
        public bool IsMainTag { get; set; }

        public TableItempropSerializable()
        { }
    }
}