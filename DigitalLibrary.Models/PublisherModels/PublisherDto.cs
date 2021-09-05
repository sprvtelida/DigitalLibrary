using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.PublisherModels
{
    public class PublisherDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
    }
}
