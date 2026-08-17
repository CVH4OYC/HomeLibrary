/* =============================================================
   03_xml_queries_examples.sql
   Примеры выборки данных из XML-поля TableOfContents (п.7 задания).
   Демонстрируются методы XML: .value(), .nodes(), .query(), .exist().
   ============================================================= */

USE [HomeLibrary];
GO

-- XML-методам (.value/.nodes/.query/.exist) требуется QUOTED_IDENTIFIER ON
SET QUOTED_IDENTIFIER ON;
GO

/* -----------------------------------------------------------------
   Пример 1. .value() — извлечь скалярное значение из XML.
   Достаём название и страницу ПЕРВОЙ главы каждой книги.
   ----------------------------------------------------------------- */
PRINT N'--- Пример 1: .value() — первая глава каждой книги ---';
SELECT
    b.Id,
    b.Title,
    b.TableOfContents.value('(/toc/chapter[1]/@title)[1]', 'NVARCHAR(300)') AS FirstChapterTitle,
    b.TableOfContents.value('(/toc/chapter[1]/@page)[1]',  'INT')           AS FirstChapterPage
FROM dbo.Books AS b
WHERE b.TableOfContents IS NOT NULL;
GO

/* -----------------------------------------------------------------
   Пример 2. .nodes() + CROSS APPLY — развернуть XML в набор строк.
   Каждая глава каждой книги становится отдельной строкой результата.
   ----------------------------------------------------------------- */
PRINT N'--- Пример 2: .nodes() — список всех глав всех книг ---';
SELECT
    b.Id,
    b.Title,
    c.value('@number', 'INT')           AS ChapterNumber,
    c.value('@title',  'NVARCHAR(300)') AS ChapterTitle,
    c.value('@page',   'INT')           AS ChapterPage
FROM dbo.Books AS b
CROSS APPLY b.TableOfContents.nodes('/toc/chapter') AS T(c)
ORDER BY b.Id, ChapterNumber;
GO

/* -----------------------------------------------------------------
   Пример 3. .nodes() двухуровневый — главы и вложенные разделы.
   ----------------------------------------------------------------- */
PRINT N'--- Пример 3: .nodes() — главы с разделами ---';
SELECT
    b.Title,
    c.value('@title', 'NVARCHAR(300)') AS ChapterTitle,
    s.value('@title', 'NVARCHAR(300)') AS SectionTitle,
    s.value('@page',  'INT')           AS SectionPage
FROM dbo.Books AS b
CROSS APPLY b.TableOfContents.nodes('/toc/chapter')   AS TC(c)
CROSS APPLY c.nodes('section')                        AS TS(s)
ORDER BY b.Title, ChapterTitle, SectionPage;
GO

/* -----------------------------------------------------------------
   Пример 4. .exist() — фильтр по наличию узла в XML.
   Находим книги, где есть глава с заголовком, содержащим 'Функции'.
   ----------------------------------------------------------------- */
PRINT N'--- Пример 4: .exist() — книги с главой про "Функции" ---';
SELECT
    b.Id,
    b.Title,
    b.Author
FROM dbo.Books AS b
WHERE b.TableOfContents.exist('/toc/chapter[contains(@title, "Функции")]') = 1;
GO

/* -----------------------------------------------------------------
   Пример 5. .query() — вернуть XML-подмножество.
   Возвращаем XML только с главами (без разделов) — переформируем узлы.
   ----------------------------------------------------------------- */
PRINT N'--- Пример 5: .query() — XML только со списком глав ---';
SELECT
    b.Id,
    b.Title,
    b.TableOfContents.query('
        <chapters>
        {
            for $c in /toc/chapter
            return <chapter>{ string($c/@title) }</chapter>
        }
        </chapters>
    ') AS ChaptersXml
FROM dbo.Books AS b
WHERE b.TableOfContents IS NOT NULL;
GO

/* -----------------------------------------------------------------
   Пример 6. Агрегация по XML — количество глав в каждой книге.
   ----------------------------------------------------------------- */
PRINT N'--- Пример 6: количество глав в каждой книге ---';
SELECT
    b.Id,
    b.Title,
    b.TableOfContents.value('count(/toc/chapter)', 'INT') AS ChapterCount
FROM dbo.Books AS b
WHERE b.TableOfContents IS NOT NULL
ORDER BY ChapterCount DESC;
GO

/* -----------------------------------------------------------------
   Пример 7. Поиск по значению внутри XML.
   Ищем книги, где какая-либо глава начинается со страницы >= 50.
   ----------------------------------------------------------------- */
PRINT N'--- Пример 7: книги, где есть глава со страницей >= 50 ---';
SELECT DISTINCT
    b.Id,
    b.Title
FROM dbo.Books AS b
WHERE b.TableOfContents.exist('/toc/chapter[@page >= 50]') = 1;
GO
