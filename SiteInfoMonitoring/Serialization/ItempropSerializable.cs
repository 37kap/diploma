using SiteInfoMonitoring.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteInfoMonitoring.Serialization
{
    [Serializable]
    public class ItempropSerializable
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public ItempropTypeEnum Type { get; set; }

        public ItempropSerializable()
        { }

    }
}