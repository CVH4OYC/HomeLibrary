/* =============================================================
   01_create_database.sql
   Создание базы данных домашней библиотеки и таблицы книг.
   Поле оглавления (TableOfContents) имеет тип XML.
   Выполнять под учётной записью с правами на CREATE DATABASE.
   ============================================================= */

IF DB_ID(N'HomeLibrary') IS NULL
BEGIN
    CREATE DATABASE [HomeLibrary];
END
GO

USE [HomeLibrary];
GO

/* Пересоздаём таблицу, если она уже есть (для повторного прогона скрипта) */
IF OBJECT_ID(N'dbo.Books', N'U') IS NOT NULL
    DROP TABLE dbo.Books;
GO

CREATE TABLE dbo.Books
(
    Id              INT             IDENTITY(1,1) NOT NULL,
    Title           NVARCHAR(300)   NOT NULL,
    Author          NVARCHAR(200)   NOT NULL,
    PublicationYear INT             NULL,
    Publisher       NVARCHAR(200)   NULL,
    ISBN            NVARCHAR(20)    NULL,
    PageCount       INT             NULL,
    Genre           NVARCHAR(100)   NULL,
    -- Оглавление книги в виде XML. Заполняется из HTML-редактора (TinyMCE),
    -- содержимое нормализуется в well-formed XML перед сохранением.
    TableOfContents XML             NULL,
    CreatedAt       DATETIME2(0)    NOT NULL CONSTRAINT DF_Books_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0)    NOT NULL CONSTRAINT DF_Books_UpdatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Books PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT CK_Books_Year CHECK (PublicationYear IS NULL OR PublicationYear BETWEEN 1400 AND 2100),
    CONSTRAINT CK_Books_Pages CHECK (PageCount IS NULL OR PageCount > 0)
);
GO

CREATE INDEX IX_Books_Author ON dbo.Books (Author);
CREATE INDEX IX_Books_Title  ON dbo.Books (Title);
GO

PRINT N'База данных HomeLibrary и таблица dbo.Books созданы.';
GO
