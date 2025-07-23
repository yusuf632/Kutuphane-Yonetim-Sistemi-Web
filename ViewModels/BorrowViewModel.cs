using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
namespace LibraryManagement.ViewModels
{
    public class BorrowViewModel
    {
        [Required]
        public int BookId { get; set; }

        [BindNever]
        public string? BookTitle { get; set; }

        [Required(ErrorMessage = "Adınız gerekli")]
        [StringLength(100, ErrorMessage = "Adınız 100 karakteri geçemez")]
        public string BorrowerName { get; set; }

        [Required(ErrorMessage = "Email'iniz gerekli")]
        [EmailAddress(ErrorMessage = "Geçersiz Email adresi")]
        public string BorrowerEmail { get; set; }

        [Required(ErrorMessage = "Telefon numaranız gerekli")]
        [Phone(ErrorMessage = "Geçersiz telefon numarası")]
        public string Phone { get; set; }
    }
}