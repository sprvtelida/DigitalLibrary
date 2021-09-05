using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.AuthorModels
{
    public class AuthorDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public Country Country { get; set; }
    }
}
