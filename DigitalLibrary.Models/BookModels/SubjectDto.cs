using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.BookModels
{
    public class SubjectDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
    }
}
