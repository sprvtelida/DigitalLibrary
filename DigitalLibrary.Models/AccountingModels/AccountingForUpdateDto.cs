using System;
using System.ComponentModel.DataAnnotations;
using DigitalLibrary.Models.Enums;

namespace DigitalLibrary.Models.AccountingModels
{
    public class AccountingForUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Status Status { get; set; }
    }
}
