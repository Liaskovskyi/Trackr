using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;

namespace Trackr.Domain.Interfaces
{
    public interface IAlbumRepository
    {
        public Task<IEnumerable<Album>> GetNewAlbumsAsync(IEnumerable<Album> albums);
        public Task SaveAlbumsToDbAsync(IEnumerable<Album> albums);
        public Task SaveAlbumsArtistsAsync(List<AlbumArtist> albums);
        public Task ExecuteInTransactionAsync(Func<Task> operation);
        public List<AlbumArtist> GetAlbumArtistsFromTrackItems(TrackItem[] trackItems);
    }
}
