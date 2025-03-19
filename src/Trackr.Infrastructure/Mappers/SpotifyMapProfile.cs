using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Trackr.Infrastructure.DTO.SpotifyPlaybackState;
using Trackr.Domain.Models;
using Trackr.Infrastructure.DTO;
using Trackr.Domain.Models.Database;

namespace Trackr.Infrastructure.Mappers
{
    public class SpotifyMapProfile : Profile
    {
        public SpotifyMapProfile()
        {
            CreateMap<SpotifyPlaybackState, PlaybackState>()
                .ForMember(dest => dest.ElapsedTime, opt => opt.MapFrom(src => src.Progress_Ms))
                .ForMember(dest => dest.IsPlaying, opt => opt.MapFrom(src => src.Is_Playing))
                .ForMember(dest => dest.CurrentTrack, opt => opt.MapFrom(src => src.Item));

            CreateMap<SpotifyAlbumDTO, Album>()
                .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Album_type))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Release_date))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Images.FirstOrDefault().Url));

            CreateMap<SpotifyTrackDTO, Track>()
                .ForMember(dest => dest.TrackId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Album.Release_date))
                .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => src.Album.Id))
                .ForMember(dest => dest.Album, opt => opt.MapFrom(src => src.Album))
                .ForMember(dest => dest.DurationMs, opt => opt.MapFrom(src => src.Duration_ms))
                .ForMember(dest => dest.ISRC, opt => opt.MapFrom(src => src.External_Ids.ISRC));

            CreateMap<SpotifyTokensDTO, Tokens>()
                .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.access_token))
                .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.refresh_token))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddSeconds(src.expires_in)));

            CreateMap<SpotifyTracksDTO, Tracks>()
                .ForMember(dest => dest.TracksArray, opt => opt.MapFrom(src => src.Items));

            CreateMap<DTO.Cursors, Domain.Models.Cursors>();

            CreateMap<SimplifiedArtist, Artist>()
                .ForMember(dest => dest.ArtistId, opt => opt.MapFrom(src => src.Id));

            CreateMap<SpotifyItemDTO, TrackItem>()
                .ForMember(dest => dest.PlayedAt, opt => opt.MapFrom(src => src.Played_at))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => src.Track.Artists));

            CreateMap<SpotifyArtist, ArtistWithGenres>()
                .ForPath(dest => dest.Artist.ArtistId, opt => opt.MapFrom(src => src.Id))
                .ForPath(dest => dest.Artist.Name, opt => opt.MapFrom(src => src.Name))
                .ForPath(dest => dest.Artist.ImageUrl, opt => opt.MapFrom(src => src.Images.FirstOrDefault().Url))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres));

        }
    }
}
