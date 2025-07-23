using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibraryManagement.Models
{
    public class BorrowRecord
    {
        [Key]
        public int BorrowRecordId { get; set; } 

        [Required]
        public int BookId { get; set; } 

        [Required(ErrorMessage = "Lütfen borçlunun adını girin")]
        public string BorrowerName { get; set; }

        [Required(ErrorMessage = "Lütfen borçlunun E-posta adresini girin")]
        [EmailAddress(ErrorMessage = "Lütfen bir E-posta adresi girin")]
        public string BorrowerEmail { get; set; }

        [Required(ErrorMessage = "Lütfen borçlunun telefon numarasını girin")]
        [Phone(ErrorMessage = "Lütfen geçerli bir telefon numarası girin")]
        public string Phone { get; set; }

        [BindNever]
        [DataType(DataType.DateTime)]
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; }

        // Navigation Properties
        [BindNever]
        public Book Book { get; set; }
    }
}