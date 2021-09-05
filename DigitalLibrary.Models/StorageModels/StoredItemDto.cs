using System;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.AccountingModels;
using DigitalLibrary.Models.BookModels;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.LibraryModels;

namespace DigitalLibrary.Models.StorageModels
{
    public class StoredItemDto
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }

        [Required]
        public int InventoryNumber { get; set; }

        [Required]
        public LibraryDto Library { get; set; }

        [Required]
        public BookDto Book { get; set; }

        public AccountingDto Accounting { get; set; }
    }
}
