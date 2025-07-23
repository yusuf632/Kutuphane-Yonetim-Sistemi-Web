using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        // Injecting the LibraryContext and Logger to interact with the database and log events.
        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // Retrieves and displays all books.
        // GET: Books
        public async Task<IActionResult> Index()
        {
            try
            {
                var books = await _context.Books
                    .Include(b => b.BorrowRecords)
                    .AsNoTracking()
                    .ToListAsync();
                return View(books);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kitaplar yüklenirken bir hata oluştu.";
                return View("Hata");
            }
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Kitap kimliği sağlanmadı.";
                return View("Bulunamadı");
            }

            try
            {
                var book = await _context.Books
                    .FirstOrDefaultAsync(m => m.BookId == id);

                if (book == null)
                {
                    TempData["ErrorMessage"] = $"{id} Kimlikle kitap bulunamadı.";
                    return View("Bulunamadı");
                }

                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kitap detayları yüklenirken bir hata oluştu.";
                return View("Hata");
            }
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // BookId and IsAvailable are not bound due to [BindNever]
                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Kitap başarıyla eklendi: {book.Title}.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Kitap eklenirken bir hata oluştu.";
                    return View(book);
                }
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided for editing.";
                return View("NotFound");
            }

            try
            {
                var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(m => m.BookId == id);

                if (book == null)
                {
                    TempData["ErrorMessage"] = $"Düzenleme için ID  {id} ile kitap bulunamadı.";
                    return View("Bulunamadı");
                }
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kitap düzenleme için yüklenirken bir hata oluştu.";
                return View("Hata");
            }
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Book book)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Kitap kimliği güncelleme için sağlanmadı.";
                return View("Bulunamadı");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBook = await _context.Books.FindAsync(id);

                    if (existingBook == null)
                    {
                        TempData["ErrorMessage"] = $"Güncellenecek  {id} ID'li kitap bulunamadı.";
                        return View("Bulunamadı");
                    }

                    // Updating fields that can be edited
                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.ISBN = book.ISBN;
                    existingBook.PublishedDate = book.PublishedDate;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Kitap başarıyla güncellendi: {book.Title}.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!BookExists(book.BookId))
                    {
                        TempData["ErrorMessage"] = $"Eşzamanlılık kontrolü sırasında  {book.BookId} kimliğine sahip kitap bulunamadı.";
                        return View("Bulunamadı");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Güncelleme sırasında bir eşzamanlılık hatası oluştu.";
                        return View("Hata");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Kitap güncellenirken bir hata oluştu.";
                    return View("Hata");
                }
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Kitap kimliği silme işlemi için sağlanmadı.";
                return View("Bulunamadı");
            }

            try
            {
                var book = await _context.Books
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.BookId == id);

                if (book == null)
                {
                    TempData["ErrorMessage"] = $"Silinecek {id} ID'li kitap bulunamadı.";
                    return View("Bulunamadı");
                }

                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kitap silinmek üzere yüklenirken bir hata oluştu.";
                return View("Hata");
            }
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"Silinecek {id} ID'li kitap bulunamadı.";
                    return View("Bulunamadı");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Kitap başarıyla silindi: {book.Title}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kitap silinirken bir hata oluştu.";
                return View("Hata");
            }
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
