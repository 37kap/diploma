using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Models
{
    public class TableItemprop : Itemprop
    {

        [Display(Name = "Является главным тегом")]
        public bool IsMainTag { get; set; }
    }
}