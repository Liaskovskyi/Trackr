using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Application.Interfaces;
using Trackr.Domain.Models;
using Trackr.Domain.Interfaces;
using Trackr.Infrastructure.Interfaces;

namespace Trackr.Application.Services
{
    public class PlaybackService : IPlaybackService
    {
        private readonly IClient _client;
        //private readonly IUserRepository _userRepository;

        public PlaybackService(IClient client) //, IUserRepository userRepositiry)
        {
            _client = client;
            //_userRepository = userRepositiry;
        }

        public async Task<PlaybackState> GetCurrentTrackAsync(int userId)
        {
            //string authToken =  await _userRepository.GetAuthTokenAsync(userId, _client.ProviderName) ?? throw new ArgumentException("No token found");
            string authToken = "BQC9eDO1AQYthWTiwia2ghtmwgxSMPCXRzyFg5rmt84GvQu-1dgVARz8fVDoIgUVvDCT2YNlcvmvjS6cGkrWoE409eZrSI4Kkw6BakXL4-eoYIMdvM4wfIiFCxIlC1TeD_Pcd8ygu-5ZnpagcZFP42Py9Qu0pWkKNYT9TYzsj6vd1FRKNcQ08aSgfxZ84A7DE00UxT0-q6RiH9jyIpWCDnZoF6z-DCwF9C52wRZ0tKE8q2Y";

            PlaybackState playBackState = await _client.GetPlaybackStateAsync(authToken);

            return playBackState;
        }
    }
}
