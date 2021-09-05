using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.Entities
{
    public class Book
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Column(TypeName = "varchar(36)")]
        public string Image { get; set; }

        [Required]
        [MaxLength(50)]
        public string ISBN { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public int Year { get; set; }

        public Language Language { get; set; }

        [MaxLength(50)]
        public string Epub { get; set; }
        [MaxLength(50)]
        public string Pdf { get; set; }
        [MaxLength(50)]
        public string Fb2 { get; set; }

        public Subject Subject { get; set; }

        public Genre Genre { get; set; }

        public Author Author { get; set; }
        public Publisher Publisher { get; set; }
    }
}
