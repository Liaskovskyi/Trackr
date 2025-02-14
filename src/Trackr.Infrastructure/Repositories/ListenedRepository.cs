using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;
using Trackr.Infrastructure.DataBase;

namespace Trackr.Infrastructure.Repositories
{
    public class ListenedRepository : IListenedRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        //private readonly ITrackRepository _trackRepository;
        //private readonly IAlbumRepository _albumRepository;
        //private readonly IArtistRepository _artistRepository;

        public ListenedRepository(AppDbContext context, 
            IMapper mapper)
            //ITrackRepository trackRepository,
            //IAlbumRepository albumRepository, 
           //IArtistRepository artistRepository)
        {
            _context = context;
            _mapper = mapper;
            //_trackRepository = trackRepository;
            //_albumRepository = albumRepository;
            //_artistRepository = artistRepository;
        }

        public async Task<Result<long>> GetLastPlayedTrackTimeFromDb(string id)
        {
            if (string.IsNullOrEmpty(id)) return Result<long>.Failure("401", "User is not authenticated.");

            DateTime time = await _context.Listened.Where(listen => listen.UserId == id).Select(l => l.ListenedAt).OrderByDescending(l => l).FirstOrDefaultAsync();
            long after = 0;
            if (time != DateTime.MinValue)
            {
                DateTime utcTime = DateTime.SpecifyKind(time, DateTimeKind.Utc);
                after = ((DateTimeOffset)utcTime).ToUnixTimeMilliseconds()+1;
            }

            return Result<long>.Success(after);
        }

        public async Task SaveListensToDbAsync(List<Listen> listens)
        {
            if (listens.Count == 0) return;
            await _context.Listened.AddRangeAsync(listens);
            await _context.SaveChangesAsync();
        }
    }
}
