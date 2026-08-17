# Домашняя библиотека — тестовое задание

Проект демонстрирует работу с одной БД MS SQL Server через хранимые процедуры из двух
разных приложений: **ASP.NET Core MVC (.NET 8)** и **ASP.NET Web Forms (.NET Framework 4.8)**.

Оглавление книги хранится в поле типа `XML` и редактируется через удобный **структурный редактор глав и разделов** в обоих приложениях.

## Структура репозитория

```
Тестовое/
├─ HomeLibrary.sln                       общее решение (оба проекта)
├─ database/
│  ├─ 01_create_database.sql             БД HomeLibrary + таблица Books (поле XML)
│  ├─ 02_stored_procedures.sql           usp_Book_Select/GetById/Insert/Update/Delete
│  ├─ 03_xml_queries_examples.sql        примеры выборки из XML (.value/.nodes/.query/.exist)
│  ├─ 04_seed_data.sql                   тестовые книги
│  └─ sample_toc.xml                     пример файла оглавления
└─ src/
   ├─ HomeLibrary.Mvc/                   ASP.NET Core MVC (.NET 8)
   └─ HomeLibrary.WebForms/              ASP.NET Web Forms (.NET Framework 4.8)
```

## 1. База данных

Использован экземпляр SQL Server (`localhost`, Windows-аутентификация).
Скрипты выполняются последовательно. Через `sqlcmd` (с флагом `-f 65001` для корректной UTF-8 кодировки):

```powershell
cd database
sqlcmd -S localhost -E -b -f 65001 -i "01_create_database.sql"
sqlcmd -S localhost -E -b -f 65001 -i "02_stored_procedures.sql"
sqlcmd -S localhost -E -b -f 65001 -i "04_seed_data.sql"
# примеры выборки из XML:
sqlcmd -S localhost -E -b -f 65001 -i "03_xml_queries_examples.sql"
```

Либо открыть скрипты в SQL Server Management Studio (SSMS) и выполнить по очереди.

### Схема таблицы `dbo.Books`

| Поле | Тип | Назначение |
|------|-----|-----------|
| Id | INT IDENTITY PK | первичный ключ |
| Title | NVARCHAR(300) | название |
| Author | NVARCHAR(200) | автор |
| PublicationYear | INT | год издания |
| Publisher | NVARCHAR(200) | издательство |
| ISBN | NVARCHAR(20) | ISBN |
| PageCount | INT | число страниц |
| Genre | NVARCHAR(100) | жанр |
| TableOfContents | XML | оглавление (структурный XML) |
| CreatedAt / UpdatedAt | DATETIME2 | служебные даты |

### Хранимые процедуры

- `usp_Book_Select` — список (с необязательным поиском по названию/автору)
- `usp_Book_GetById` — карточка по Id
- `usp_Book_Insert` — создание (возвращает новый Id через OUTPUT-параметр)
- `usp_Book_Update` — изменение
- `usp_Book_Delete` — удаление

### Примеры выборки из XML (`03_xml_queries_examples.sql`)

- `.value()` — извлечение скалярных значений (заголовок/страница главы);
- `.nodes()` + `CROSS APPLY` — развёртывание глав/разделов в строки;
- `.exist()` — фильтрация книг по наличию узла;
- `.query()` — получение XML-подмножества;
- агрегаты по XML (количество глав).

## 2. Строка подключения

Оба приложения используют одну строку подключения:

```
Server=localhost;Database=HomeLibrary;Trusted_Connection=True;TrustServerCertificate=True;
```

- **MVC**: `src/HomeLibrary.Mvc/appsettings.json`, секция `ConnectionStrings:HomeLibrary`.
- **Web Forms**: `src/HomeLibrary.WebForms/Web.config`, секция `<connectionStrings>`.

Если инстанс именованный (например, `SQLEXPRESS`), укажите `Server=localhost\SQLEXPRESS`.

## 3. Запуск приложений

### ASP.NET Core MVC

```powershell
cd src/HomeLibrary.Mvc
dotnet run
```

При старте сразу открывается каталог книг (`/` маршрутизируется в `Books/Index`).

### ASP.NET Web Forms

1. Открыть `HomeLibrary.sln` в Visual Studio 2022.
2. Назначить проект `HomeLibrary.WebForms` стартовым (Set as Startup Project).
3. Запустить через **IIS Express** (клавиша `F5`). Стартовая страница сразу перенаправляет в список книг (`Books/List.aspx`).

Сборка из командной строки:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" `
  "src\HomeLibrary.WebForms\HomeLibrary.WebForms.csproj" `
  /p:Configuration=Debug `
  "/p:VSToolsPath=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Microsoft\VisualStudio\v17.0"
```

## 4. Реализованный функционал (в обоих приложениях)

- **Каталог книг**: таблица со списком, поиском по названию и автору.
- **Стартовый экран**: приложения сразу открывают функционал библиотеки без лишних пустых вкладок.
- **Карточка книги**: просмотр всех реквизитов и форматированное оглавление со структурой глав, разделов и номеров страниц.
- **Создание и редактирование**:
  - Валидация обязательных полей (название, автор).
  - Интерактивный структурный редактор оглавления (добавление/удаление глав и разделов, указание страниц).
- **Удаление книги**.
- **Архитектура**: все обращения к БД выполняются исключительно через хранимые процедуры (ADO.NET, `CommandType.StoredProcedure`).
- **Кодировка**: все файлы проектов и конфигурации настроены на UTF-8 с BOM и `<globalization>`, что исключает проблемы с кириллицей на любых версиях Windows.

## Про оглавление и XML

Оглавление сохраняется в столбец типа `XML` в виде структурированного документа:

```xml
<toc>
    <chapter number="1" title="Чистый код" page="15">
        <section title="Что такое чистый код" page="18" />
    </chapter>
    <chapter number="2" title="Содержательные имена" page="36" />
</toc>
```

Класс `TableOfContentsHelper` в обоих проектах обеспечивает:
1. `BuildXml` — генерацию XML из элементов структурного редактора.
2. `ParseChapters` — разбор хранимого XML обратно в модели глав и разделов для формы редактирования.
3. `ToDisplayHtml` — преобразование XML во вложенный читаемый HTML-список для страницы карточки книги.
