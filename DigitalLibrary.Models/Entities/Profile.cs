using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.Entities
{
    public class Profile
    {
        public Guid Id { get; set; }

        public Guid Image { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(12)]
        public string IIN { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [Required]
        public City City { get; set; }

        public Library RegisteredLibrary { get; set; }
    }
}
