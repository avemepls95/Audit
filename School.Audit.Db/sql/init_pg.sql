create table "AuditItem"
(
    "TargetType"          text                     not null,
    "KeyPropertyValue"    text                     not null,
    "OperationType"       text                     not null,
    "ChangedPropertyName" text,
    "OldValue"            text,
    "NewValue"            text,
    "Date"                timestamp with time zone not null,
    "Id"                  bigserial
        constraint audititem_pk
            primary key
);