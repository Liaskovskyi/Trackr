using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Infrastructure.DTO
{
    public class SpotifyImageDTO
    {
        public string? Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
