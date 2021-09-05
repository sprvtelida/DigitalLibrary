using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.Entities
{
    public class Author
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public Country Country { get; set; }

        private ICollection<Book> Books { get; set; }
    }
}
