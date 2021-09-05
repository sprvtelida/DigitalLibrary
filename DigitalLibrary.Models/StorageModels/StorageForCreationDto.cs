using System;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Models.StorageModels
{
    public class StorageForCreationDto
    {
        [Required]
        public DateTime ArrivalDate { get; set; }

        [Required]
        public int InventoryNumber { get; set; }

        [Required]
        public Guid LibraryId { get; set; }

        [Required]
        public Guid BookId { get; set; }
    }
}
