using System;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.BookModels;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Enums;
using DigitalLibrary.Models.LibraryModels;
using DigitalLibrary.Models.ProfileModels;
using DigitalLibrary.Models.StorageModels;

namespace DigitalLibrary.Models.AccountingModels
{
    public class AccountingDto
    {
        public Guid Id { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        public DateTime IssueDate { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        public DateTime DateReturned { get; set; }

        public LibraryDto Library { get; set; }
        public BookDto Book { get; set; }
        public StoredItemDto Storage { get; set; }
        public ProfileDto Profile { get; set; }
    }
}
