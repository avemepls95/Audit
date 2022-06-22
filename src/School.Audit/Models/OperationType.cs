namespace School.Audit.Models
{
    /// <summary>
    /// Тип операции над объектом, в результате которого создался элемент аудита.
    /// </summary>
    public enum OperationType
    {
        None = 0,
        Create,
        Modify,
        Delete
    }   
}
