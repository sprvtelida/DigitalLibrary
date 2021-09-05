using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.LibraryModels
{
    public class LibraryForCreationDto
    {
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
