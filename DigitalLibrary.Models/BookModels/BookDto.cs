using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalLibrary.Models.AuthorModels;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Enums;
using DigitalLibrary.Models.PublisherModels;

namespace DigitalLibrary.Models.BookModels
{
    public class BookDto
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

        public SubjectDto Subject { get; set; }

        public GenreDto Genre { get; set; }

        public AuthorDto Author { get; set; }
        public PublisherDto Publisher { get; set; }

        public bool IsInStorage { get; set; } = false;
    }
}
