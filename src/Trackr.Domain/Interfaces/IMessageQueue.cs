using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Interfaces
{
    public interface IMessageQueue
    {
        public Task InitializeConnectionAsync();
        //public Task SendMessageAsync(string message, string queueName);
        public Task ReceiveMessageAsync(Func<string, Task> messageHandler);
        public Task SendDelayedMessageAsync(string message);
    }
}
