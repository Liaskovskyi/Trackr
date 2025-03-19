using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Trackr.Application.Interfaces;
using Trackr.Domain.Interfaces;

namespace Trackr.Application.Services
{
    public class MessageConsumerService : BackgroundService
    {
        public readonly IMessageQueue _mq;
        private readonly IServiceProvider _serviceProvider;  

        public MessageConsumerService(IMessageQueue mq, IServiceProvider serviceProvider)
        {
            _mq = mq;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _mq.ReceiveMessageAsync( async (message) =>
            {
                string userId = message;

                using (var scope = _serviceProvider.CreateScope())
                {
                    var trackService = scope.ServiceProvider.GetRequiredService<ITrackService>();

                    var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims));

                    await trackService.UpdateLastPlayedTracks(claimsPrincipal);
                    
                }
            });
            
        }
    }
}
