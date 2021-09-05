using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.BookModels
{
    public class GenreForCreationDto
    {
        private string _title;

        [Required]
        [MaxLength(100)]
        public string Title
        {
            get { return _title; }
            set { _title = value.Trim(); }
        }
    }
}
