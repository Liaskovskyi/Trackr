using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Infrastructure.DTO
{
    internal class SpotifyErrorResponse
    {
        public SpotifyError Error { get; set; } = new SpotifyError();
    }

    internal class SpotifyError
    {
        public int Status { get; set; }
        public string? Message { get; set; }
    }
}
