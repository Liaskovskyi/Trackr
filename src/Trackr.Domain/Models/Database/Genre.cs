using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models.Database
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }
        public string? GenreName { get; set; }
    }
}
