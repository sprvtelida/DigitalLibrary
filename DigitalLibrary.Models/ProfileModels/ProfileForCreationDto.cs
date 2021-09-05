using System;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Enums;
using DigitalLibrary.Models.LibraryModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace DigitalLibrary.Models.ProfileModels
{
    public class ProfileForCreationDto
    {
        //[Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        //[Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        //[Required]
        [MaxLength(12)]
        public string IIN { get; set; }

        //[Required]
        [MaxLength(200)]
        public string Address { get; set; }

        //[Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        //[Required]
        public City City { get; set; }

        //[Required]
        public Guid RegisteredLibraryId { get; set; }
    }
}
