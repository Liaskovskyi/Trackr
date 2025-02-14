using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models.Database;
using Trackr.Domain.Models;

namespace Trackr.Domain.Interfaces
{
    public interface ITrackRepository
    {
        public Task<IEnumerable<Track>> GetNewTracksAsync(IEnumerable<Track> tracks);
        public Task SaveTracksToDbAsync(IEnumerable<Track> tracks);
        public Task SaveTracksArtistsAsync(List<TrackArtist> tracks);
        public Task ExecuteInTransactionAsync(Func<Task> operation);
        public List<TrackArtist> GetTracksArtistsFromTrackItems(TrackItem[] trackItems);
    }
}
