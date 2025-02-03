using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Application.DTOs;
using Trackr.Application.Interfaces;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;

namespace Trackr.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(RegisterDTO registerInfo)
        {
            var userInfo = _mapper.Map<User>(registerInfo);
            bool registered = await _userRepository.RegisterUserAsync(userInfo);

            return registered;
        }
    }
}
