using SiteInfoMonitoring.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteInfoMonitoring.Serialization
{
    [Serializable]
    public class TableSerializable
    {
        public List<TableItempropSerializable> TableItemprops { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public TableTypeEnum Type { get; set; }

        public TableSerializable()
        {
        }
    }
}