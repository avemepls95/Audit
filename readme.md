# Аудит

Библиотека аудита позволяет отслеживать и сохранять изменения сущностей.
Данные аудита:
- Идентификатор самого элемента аудита
- Тип аудируемой объекта
- Значение ключа аудируемого объекта
- Имя измененного свойства
- Старое значение измененного свойства
- Новое значение измененного свойства
- Тип операции над объектом, в результате которого создался элемент аудита (создание, изменение, удаление)
- Дата и время операции

## Конфигурация
> На данный момент сбор аудита и его "сохранение" реализован только для БД посредством ef core.
> Для работы с другимм источниками требуется релизовать интерфейс `IChangesProvider`


Для настройки аудита какой-либо сущности потребуется:
- Подключить библиотеку `School.Audit.Db`
- Создать таблицу в БД (скрипт для postgresql находится в `src\School.Audit.Db\sql`)
- Указать конфигурацию элемента аудита `DbAuditConfiguration` в `OnModelCreating`
- Вызвать `IChangesDbTrackingManager.AddChanges` перед сохранением в БД
- Указать сущности, ее ключ и свойства, которые необходимо аудировать

## Пример
Регистрация осуществляется через `IServiceCollection`
```cs 
services.AddDbAudit<MyDbContext>(builder =>
    {
        builder
            .Add<SomeClass>(c => c.Id)
            .AddProperties(c =>
                c => c.IntProperty,
                c => c.StringProperty,
                c => c.BoolProperty
            );
    }
);
```
Добавление аудируемых объектов/полей в контекст:
```cs 
internal class MyDbContext : DbContext
{
    private readonly IChangesDbTrackingManager _changesDbTrackingManager;
    ...
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _changesDbTrackingManager.AddChanges();

        return base.SaveChangesAsync(cancellationToken);
    }
}
```
Указание конфигурации элемента аудита
```cs 
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	...
	modelBuilder.ApplyConfiguration(new DbAuditConfiguration());
}
```