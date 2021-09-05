using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.BookModels
{
    public class BookForCreationDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(36)]
        public string Image { get; set; }

        [Required]
        [MaxLength(50)]
        public string ISBN { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public int Year { get; set; }

        public Language Language { get; set; }

        [MaxLength(36)]
        public string GenreId { get; set; }

        [MaxLength(36)]
        public string SubjectId { get; set; }

        [MaxLength(36)]
        public string AuthorId { get; set; }

        [MaxLength(36)]
        public string PublisherId { get; set; }
    }
}
