using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        [BindNever]
        public int BookId { get; set; } 

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Başlık 100 karakteri geçemez.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Yazar alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Yazar adı 100 karakteri geçemez.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "ISBN alanı zorunludur.")]
        [RegularExpression(@"^\d{3}-\d{10}$", ErrorMessage = "ISBN XXX-XXXXXXXXXX formatında olmalıdır.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Yayınlanma tarihi alanı zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Yayınlanma tarihi")]
        public DateTime PublishedDate { get; set; }

        [BindNever]
        [Display(Name = "Mevcut")]
        public bool IsAvailable { get; set; } = true; 

        // Navigation Property
        [BindNever]
        public ICollection<BorrowRecord>? BorrowRecords { get; set; }
    }
}