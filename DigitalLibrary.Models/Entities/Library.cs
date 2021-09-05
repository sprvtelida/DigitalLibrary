using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.Entities
{
    public class Library
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

        public ICollection<Profile> Profiles { get; set; }
    }
}
