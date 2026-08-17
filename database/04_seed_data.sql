/* =============================================================
   04_seed_data.sql
   Тестовые данные. Оглавление кладём в XML-поле.
   ============================================================= */

USE [HomeLibrary];
GO

DELETE FROM dbo.Books;
-- TRUNCATE сбрасывает IDENTITY так, что следующая вставка начнётся с 1
TRUNCATE TABLE dbo.Books;
GO

DECLARE @newId INT;

EXEC dbo.usp_Book_Insert
    @Title = N'Чистый код',
    @Author = N'Роберт Мартин',
    @PublicationYear = 2008,
    @Publisher = N'Питер',
    @ISBN = N'978-5-4461-0960-9',
    @PageCount = 464,
    @Genre = N'Программирование',
    @TableOfContents = N'<toc>
        <chapter number="1" title="Чистый код" page="15">
            <section title="Что такое чистый код" page="18" />
        </chapter>
        <chapter number="2" title="Содержательные имена" page="36" />
        <chapter number="3" title="Функции" page="52">
            <section title="Компактность" page="53" />
            <section title="Один уровень абстракции" page="57" />
        </chapter>
    </toc>',
    @NewId = @newId OUTPUT;

EXEC dbo.usp_Book_Insert
    @Title = N'Мастер и Маргарита',
    @Author = N'Михаил Булгаков',
    @PublicationYear = 1967,
    @Publisher = N'АСТ',
    @ISBN = N'978-5-17-089621-3',
    @PageCount = 512,
    @Genre = N'Роман',
    @TableOfContents = N'<toc>
        <chapter number="1" title="Часть первая" page="5">
            <section title="Никогда не разговаривайте с неизвестными" page="7" />
            <section title="Понтий Пилат" page="24" />
        </chapter>
        <chapter number="2" title="Часть вторая" page="250">
            <section title="Маргарита" page="252" />
        </chapter>
    </toc>',
    @NewId = @newId OUTPUT;

EXEC dbo.usp_Book_Insert
    @Title = N'Алгоритмы. Построение и анализ',
    @Author = N'Томас Кормен',
    @PublicationYear = 2009,
    @Publisher = N'Вильямс',
    @ISBN = N'978-5-8459-1794-2',
    @PageCount = 1296,
    @Genre = N'Информатика',
    @TableOfContents = N'<toc>
        <chapter number="1" title="Роль алгоритмов в вычислениях" page="27" />
        <chapter number="2" title="Начинаем работу" page="39">
            <section title="Сортировка вставками" page="40" />
            <section title="Анализ алгоритмов" page="48" />
        </chapter>
        <chapter number="3" title="Рост функций" page="66" />
    </toc>',
    @NewId = @newId OUTPUT;

GO

SELECT Id, Title, Author, PublicationYear FROM dbo.Books;
GO

PRINT N'Тестовые данные добавлены.';
GO
