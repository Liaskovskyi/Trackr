using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models
{
    public class PlaybackState
    {
        public Track CurrentTrack { get; set; }
        public int ElapsedTime { get; set; }
        public bool IsPlaying { get; set; }
    }
}
