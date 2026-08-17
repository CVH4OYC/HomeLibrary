/* =============================================================
   02_stored_procedures.sql
   Хранимые процедуры CRUD + выборка для таблицы dbo.Books.
   Все обращения из приложений выполняются ТОЛЬКО через них.
   ============================================================= */

USE [HomeLibrary];
GO

/* -------------------- SELECT (список) -------------------- */
IF OBJECT_ID(N'dbo.usp_Book_Select', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Book_Select;
GO
CREATE PROCEDURE dbo.usp_Book_Select
    @Search NVARCHAR(300) = NULL   -- необязательный поиск по названию/автору
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Title,
        Author,
        PublicationYear,
        Publisher,
        ISBN,
        PageCount,
        Genre,
        TableOfContents,
        CreatedAt,
        UpdatedAt
    FROM dbo.Books
    WHERE (@Search IS NULL
           OR Title  LIKE N'%' + @Search + N'%'
           OR Author LIKE N'%' + @Search + N'%')
    ORDER BY Title;
END
GO

/* -------------------- SELECT по Id (карточка) -------------------- */
IF OBJECT_ID(N'dbo.usp_Book_GetById', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Book_GetById;
GO
CREATE PROCEDURE dbo.usp_Book_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Title,
        Author,
        PublicationYear,
        Publisher,
        ISBN,
        PageCount,
        Genre,
        TableOfContents,
        CreatedAt,
        UpdatedAt
    FROM dbo.Books
    WHERE Id = @Id;
END
GO

/* -------------------- INSERT -------------------- */
IF OBJECT_ID(N'dbo.usp_Book_Insert', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Book_Insert;
GO
CREATE PROCEDURE dbo.usp_Book_Insert
    @Title           NVARCHAR(300),
    @Author          NVARCHAR(200),
    @PublicationYear INT           = NULL,
    @Publisher       NVARCHAR(200) = NULL,
    @ISBN            NVARCHAR(20)  = NULL,
    @PageCount       INT           = NULL,
    @Genre           NVARCHAR(100) = NULL,
    @TableOfContents XML           = NULL,
    @NewId           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Books
        (Title, Author, PublicationYear, Publisher, ISBN, PageCount, Genre, TableOfContents, CreatedAt, UpdatedAt)
    VALUES
        (@Title, @Author, @PublicationYear, @Publisher, @ISBN, @PageCount, @Genre, @TableOfContents, SYSUTCDATETIME(), SYSUTCDATETIME());

    SET @NewId = SCOPE_IDENTITY();
END
GO

/* -------------------- UPDATE -------------------- */
IF OBJECT_ID(N'dbo.usp_Book_Update', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Book_Update;
GO
CREATE PROCEDURE dbo.usp_Book_Update
    @Id              INT,
    @Title           NVARCHAR(300),
    @Author          NVARCHAR(200),
    @PublicationYear INT           = NULL,
    @Publisher       NVARCHAR(200) = NULL,
    @ISBN            NVARCHAR(20)  = NULL,
    @PageCount       INT           = NULL,
    @Genre           NVARCHAR(100) = NULL,
    @TableOfContents XML           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Books
    SET
        Title           = @Title,
        Author          = @Author,
        PublicationYear = @PublicationYear,
        Publisher       = @Publisher,
        ISBN            = @ISBN,
        PageCount       = @PageCount,
        Genre           = @Genre,
        TableOfContents = @TableOfContents,
        UpdatedAt       = SYSUTCDATETIME()
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

/* -------------------- DELETE -------------------- */
IF OBJECT_ID(N'dbo.usp_Book_Delete', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Book_Delete;
GO
CREATE PROCEDURE dbo.usp_Book_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Books
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

PRINT N'Хранимые процедуры созданы: usp_Book_Select, usp_Book_GetById, usp_Book_Insert, usp_Book_Update, usp_Book_Delete.';
GO
