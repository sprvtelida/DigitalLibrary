using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.Entities
{
    public class Publisher
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }


        public ICollection<Book> Books { get; set; }
    }
}
