using System;
using System.Collections.Generic;
using System.Text;

namespace NapsterScrappingBusiness.Dtos
{
    public class GenderSubGenreDto
    {
        public string Gender { get; set; }
        public List<SubGenreDetail> SubGenre { get; set; } = new List<SubGenreDetail>();
    }
    public class SubGenreDetail
    {
        public string SubGenreName { get; set; }
        public List<string> SubGenreDetails { get; set; }
    }
}
