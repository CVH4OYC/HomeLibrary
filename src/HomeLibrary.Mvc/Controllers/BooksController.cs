using HomeLibrary.Mvc.Models;
using HomeLibrary.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Mvc.Controllers;

/// <summary>
/// CRUD книг через хранимые процедуры: список, карточка, создание, редактирование, удаление.
/// </summary>
public class BooksController : Controller
{
    private readonly BookRepository _repository;

    public BooksController(BookRepository repository)
    {
        _repository = repository;
    }

    // GET: /Books — список с поиском
    public async Task<IActionResult> Index(string? search)
    {
        var books = await _repository.GetAllAsync(search);
        ViewData["Search"] = search;
        return View(books);
    }

    // GET: /Books/Details/5 — карточка (просмотр)
    public async Task<IActionResult> Details(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        ViewData["TocHtml"] = TableOfContentsHelper.ToDisplayHtml(book.TableOfContents);
        return View(book);
    }

    // GET: /Books/Create
    public IActionResult Create()
    {
        ViewData["Chapters"] = new List<TocChapter>();
        return View(new Book());
    }

    // POST: /Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book, List<TocChapter>? chapters)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Chapters"] = chapters ?? new List<TocChapter>();
            return View(book);
        }

        book.TableOfContents = TableOfContentsHelper.BuildXml(chapters);
        var newId = await _repository.InsertAsync(book);
        TempData["Message"] = $"Книга добавлена (Id = {newId}).";
        return RedirectToAction(nameof(Details), new { id = newId });
    }

    // GET: /Books/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        ViewData["Chapters"] = TableOfContentsHelper.ParseChapters(book.TableOfContents);
        return View(book);
    }

    // POST: /Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book, List<TocChapter>? chapters)
    {
        if (id != book.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Chapters"] = chapters ?? new List<TocChapter>();
            return View(book);
        }

        book.TableOfContents = TableOfContentsHelper.BuildXml(chapters);
        var rows = await _repository.UpdateAsync(book);
        if (rows == 0)
        {
            return NotFound();
        }

        TempData["Message"] = "Изменения сохранены.";
        return RedirectToAction(nameof(Details), new { id = book.Id });
    }

    // GET: /Books/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: /Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteAsync(id);
        TempData["Message"] = "Книга удалена.";
        return RedirectToAction(nameof(Index));
    }
}
