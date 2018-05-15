using SiteInfoMonitoring.Core.Enums;
using System.Collections.Generic;

namespace SiteInfoMonitoring.Models
{
    public class Table
    {
        public List<TableItemprop> TableItemprops;
        public bool IsOk = false;
        public string Name = "Таблица";
        public User ResponsibleUser;
        public TableTypeEnum Type = TableTypeEnum.Required;
        public int RowCount = 0;
        public Table()
        {
            TableItemprops = new List<TableItemprop>();
        }
        public void AddTableItemprop(TableItemprop tip)
        {
            TableItemprops.Add(tip);
        }
        public void AddItemprop(Itemprop ip)
        {
            var tip = new TableItemprop();
            tip.Description = ip.Description;
            tip.Type = ip.Type;
            tip.Value = ip.Value;
            TableItemprops.Add(tip);
        }
    }
}