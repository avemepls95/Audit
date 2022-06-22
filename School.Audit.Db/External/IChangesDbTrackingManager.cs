namespace School.Audit.Db.External
{
    /// <summary>
    /// Менеджер, работающий с изменениями сущностей в контексте БД.
    /// </summary>
    public interface IChangesDbTrackingManager
    {
        /// <summary>
        /// Добавляет текущие изменения аудита в контекст БД.
        /// </summary>
        /// <remarks>
        /// Не производит сохранение в БД. 
        /// </remarks>
        void AddChanges();
    }
}