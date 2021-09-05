using System;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Enums;
using DigitalLibrary.Models.LibraryModels;
using Microsoft.AspNetCore.Http;

namespace DigitalLibrary.Models.ProfileModels
{
    public class ProfileDto
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

        public LibraryDto RegisteredLibrary { get; set; }

        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
