using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.AccountingModels
{
    public class AccountingForCreationDto
    {
        [Required]
        public Guid LibraryId { get; set; }
        [Required]
        public Guid BookId { get; set; }
        [Required]
        public DateTime ReturnDate { get; set; }
    }
}
