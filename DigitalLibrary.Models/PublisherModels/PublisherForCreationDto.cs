using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.PublisherModels
{
    public class PublisherForCreationDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
    }
}
