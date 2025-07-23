﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
namespace LibraryManagement.ViewModels
{
    public class ReturnViewModel
    {
        [Required]
        public int BorrowRecordId { get; set; }

        [BindNever]
        public string? BookTitle { get; set; }
        [BindNever]
        public string? BorrowerName { get; set; }
        [BindNever]
        public DateTime? BorrowDate { get; set; }
    }
}