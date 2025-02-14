using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;

namespace Trackr.Domain.Interfaces
{
    public interface IListenedRepository
    {
        public Task<Result<long>> GetLastPlayedTrackTimeFromDb(string id);
        //public Task SaveReceivedTracksToDbAsync(string id, Tracks tracks);
        public Task SaveListensToDbAsync(List<Listen> listens);
    }
}
