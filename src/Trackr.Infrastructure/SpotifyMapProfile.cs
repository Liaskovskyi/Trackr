using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Trackr.Infrastructure.DTO.SpotifyPlaybackState;
using Trackr.Domain.Models;
using Trackr.Infrastructure.DTO;

namespace Trackr.Infrastructure
{
    public class SpotifyMapProfile : Profile
    {
        public SpotifyMapProfile() {
            CreateMap<SpotifyPlaybackState, PlaybackState>()
                .ForMember(dest => dest.ElapsedTime, opt => opt.MapFrom(src => src.Progress_Ms))
                .ForMember(dest => dest.IsPlaying, opt => opt.MapFrom(src => src.Is_Playing))
                .ForMember(dest => dest.CurrentTrack, opt => opt.MapFrom(src => src.Item));


            CreateMap<SpotifyTrack, Track>()
                .ForMember(dest => dest.TrackId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Album.Release_date))
                .ForPath(dest => dest.AlbumId, opt => opt.MapFrom(src => src.Album.Id))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration_ms));
        }
    }
}
