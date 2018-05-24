using SiteInfoMonitoring.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Models
{
    public class Table
    {
        public List<TableItemprop> TableItemprops;
        public bool IsOk = false;

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Заполните поле")]
        public string Name { get; set; }
        public User ResponsibleUser { get; set; }

        [Display(Name = "Тип")]
        [Required(ErrorMessage = "Заполните поле")]
        public TableTypeEnum Type { get; set; }
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