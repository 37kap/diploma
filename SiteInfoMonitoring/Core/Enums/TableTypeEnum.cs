namespace SiteInfoMonitoring.Core.Enums
{
    public enum TableTypeEnum
    {
        /// <summary>
        /// Обязательная таблица.
        /// </summary>
        Required = 0,
        /// <summary>
        /// Необязательная таблица, отображаемая только при условии наличия каких-либо данных.
        /// </summary>
        Optional = 1
    }
}