using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.Entities
{
    public class BookItem
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }

        [Required]
        public int InventoryNumber { get; set; }

        [Required]
        public Library Library { get; set; }

        [Required]
        public Book Book { get; set; }
    }
}
