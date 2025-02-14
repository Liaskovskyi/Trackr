using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models.Database
{
    public class Album
    {
        [Key]
        public string? AlbumId { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? ImageUrl { get; set; }
    }
}
