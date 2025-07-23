using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class BorrowController : Controller
    {
        private readonly LibraryContext _context;

        public BorrowController(LibraryContext context)
        {
            _context = context;
        }

        // Displays the borrow form for a specific book.
        // GET: Borrow/Create/5
        public async Task<IActionResult> Create(int? bookId)
        {
            if (bookId == null || bookId == 0)
            {
                TempData["ErrorMessage"] = "Ödünç almak için kitap kimliği verilmemiştir.";
                return View("Bulunamadı");
            }

            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"Ödünç almak için {bookId} kimliğine sahip kitap bulunamadı.";
                    return View("Bulunamadı");
                }

                if (!book.IsAvailable)
                {
                    TempData["ErrorMessage"] = $"'{book.Title}' kitabı şu anda ödünç alınamıyor.";
                    return View("Mevcut değil");
                }

                var borrowViewModel = new BorrowViewModel
                {
                    BookId = book.BookId,
                    BookTitle = book.Title
                };

                return View(borrowViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ödünç alma formu yüklenirken bir hata oluştu.";
                return View("Hata");
            }
        }

        // Processes the borrowing action, creates a BorrowRecord, updates the book's availability
        // POST: Borrow/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BorrowViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var book = await _context.Books.FindAsync(model.BookId);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"Ödünç alınacak {model.BookId} kimliğine sahip kitap bulunamadı.";
                    return View("Bulunamadı");
                }

                if (!book.IsAvailable)
                {
                    TempData["ErrorMessage"] = $"'{book.Title}' kitabı zaten ödünç alınmış.";
                    return View("Mevcut değil");
                }

                var borrowRecord = new BorrowRecord
                {
                    BookId = book.BookId,
                    BorrowerName = model.BorrowerName,
                    BorrowerEmail = model.BorrowerEmail,
                    Phone = model.Phone,
                    BorrowDate = DateTime.UtcNow
                };

                // Update the book's availability
                book.IsAvailable = false;

                _context.BorrowRecords.Add(borrowRecord);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Kitabı başarıyla ödünç alındı: {book.Title}.";
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ödünç alma eylemi işlenirken bir hata oluştu.";
                return View("Hata");
            }
        }

        // Displays the return confirmation for a specific borrow record
        // GET: Borrow/Return/5
        public async Task<IActionResult> Return(int? borrowRecordId)
        {
            if (borrowRecordId == null || borrowRecordId == 0)
            {
                TempData["ErrorMessage"] = "Ödünç Kayıt Kimliği iade için sağlanmadı.";
                return View("Bulunamadı");
            }

            try
            {
                var borrowRecord = await _context.BorrowRecords
                    .Include(br => br.Book)
                    .FirstOrDefaultAsync(br => br.BorrowRecordId == borrowRecordId);

                if (borrowRecord == null)
                {
                    TempData["ErrorMessage"] = $"ID {borrowRecordId} ile döndürülecek ödünç kaydı bulunamadı.";
                    return View("Bulunamadı");
                }

                if (borrowRecord.ReturnDate != null)
                {
                    TempData["ErrorMessage"] = $"'{borrowRecord.Book.Title}' için ödünç alma kaydı zaten döndürüldü.";
                    return View("Çoktan iade edildi");
                }

                var returnViewModel = new ReturnViewModel
                {
                    BorrowRecordId = borrowRecord.BorrowRecordId,
                    BookTitle = borrowRecord.Book.Title,
                    BorrowerName = borrowRecord.BorrowerName,
                    BorrowDate = borrowRecord.BorrowDate
                };

                return View(returnViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İade onayı yüklenirken bir hata oluştu.";
                return View("Hata");
            }
        }

        // Processes the return action, updates the BorrowRecord with the return date, updates the book's availability
        // POST: Borrow/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(ReturnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var borrowRecord = await _context.BorrowRecords
                    .Include(br => br.Book)
                    .FirstOrDefaultAsync(br => br.BorrowRecordId == model.BorrowRecordId);

                if (borrowRecord == null)
                {
                    TempData["ErrorMessage"] = $"Döndürülecek {model.BorrowRecordId} kimliğine sahip ödünç kaydı bulunamadı.";
                    return View("Bulunamadı");
                }

                if (borrowRecord.ReturnDate != null)
                {
                    TempData["ErrorMessage"] = $"'{borrowRecord.Book.Title}' için ödünç alma kaydı zaten döndürüldü.";
                    return View("Çoktan iade edildi");
                }

                // Update the borrow record
                borrowRecord.ReturnDate = DateTime.UtcNow;

                // Update the book's availability
                borrowRecord.Book.IsAvailable = true;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Kitabı başarıyla iade edildi: {borrowRecord.Book.Title}.";
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İade eylemi işlenirken bir hata oluştu.";
                return View("Hata");
            }
        }
    }
}