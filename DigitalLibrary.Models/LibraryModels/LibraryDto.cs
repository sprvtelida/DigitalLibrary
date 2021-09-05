using System;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.LibraryModels
{
    public class LibraryDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        public City City { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
    }
}
