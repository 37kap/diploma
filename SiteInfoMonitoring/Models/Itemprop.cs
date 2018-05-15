using SiteInfoMonitoring.Core.Enums;

namespace SiteInfoMonitoring.Models
{
    public class Itemprop
    {
        public string Value;
        public string Description = "Описание отсутствует";
        public bool IsExist = false;
        public int Count = 0;
        public User ResponsibleUser;
        public ItempropTypeEnum Type = ItempropTypeEnum.Required;
    }
}