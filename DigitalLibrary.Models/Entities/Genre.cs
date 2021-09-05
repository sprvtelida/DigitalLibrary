using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.Entities
{
    public class Genre
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
