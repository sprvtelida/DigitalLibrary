using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.Entities
{
    public class Accounting
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

        public Library Library { get; set; }

        public Book Book { get; set; }
        public BookItem StoredItem { get; set; }
        public Profile Profile { get; set; }
    }
}
