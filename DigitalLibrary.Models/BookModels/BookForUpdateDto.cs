using System;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.AuthorModels;
using DigitalLibrary.Models.Enums;
using DigitalLibrary.Models.PublisherModels;

namespace DigitalLibrary.Models.BookModels
{
    public class BookForUpdateDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

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
