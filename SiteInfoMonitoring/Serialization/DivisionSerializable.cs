using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteInfoMonitoring.Serialization
{
    [Serializable]
    public class DivisionSerializable
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public List<ItempropSerializable> Data { get; set; }
        public List<TableSerializable> Tables { get; set; }

        public DivisionSerializable()
        {
        }
    }
}