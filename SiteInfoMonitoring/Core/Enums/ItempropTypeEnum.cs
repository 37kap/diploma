namespace SiteInfoMonitoring.Core.Enums
{
    public enum ItempropTypeEnum
    {
        /// <summary>
        /// Обязательный атрибут.
        /// </summary>
        Required = 0,
        /// <summary>
        /// Атрибут необязательных параметров.
        /// </summary>
        Optional = 1,
        /// <summary>
        /// Атрибут обязательных параметров, имеющих несколько значений.
        /// </summary>
        Selective = 2
    }
}