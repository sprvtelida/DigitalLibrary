using System;
using System.Collections.Generic;

namespace DigitalLibrary.Models.Parameters
{
    public class BookParameters : QueryStringParameters
    {
        public BookParameters()
        {
            OrderBy = "Title";
        }

        public string LibraryId { get; set; } = null;

        public bool OnlyInStorage { get; set; } = false;
        public int MinYear { get; set; } = 0;
        public int MaxYear { get; set; } = DateTime.Now.Year;

        public string SearchField { get; set; } = "Title";
        public string SearchTerm { get; set; }

        public int MinPages { get; set; } = 0;
        public int MaxPages { get; set; } = 0;

        public IEnumerable<string> GenreIds { get; set; } = new List<string>();
        public IEnumerable<string> SubjectIds { get; set; } = new List<string>();
    }
}
