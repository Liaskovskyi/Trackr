using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;

namespace Trackr.Infrastructure.Clients
{
    public class RabbitMQClient : IDisposable, IMessageQueue
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;

        public RabbitMQClient(string hostName, string userName, string password)
        {
            _hostName = hostName;
            _userName = userName;
            _password = password;
        }

        public async Task InitializeConnectionAsync()
        {
            if (_connection != null) return; 

            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't create connection to MQ: {ex}");
            }

            await SetupQueues();
        }


        public async Task SendDelayedMessageAsync(string message)
        {
            if (_channel == null)
                await InitializeConnectionAsync();

            //await _channel!.QueueDeclareAsync("delayed-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var properties = new BasicProperties
            {
                Persistent = true
            };
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "delayed-queue",mandatory:false, basicProperties: properties, body: body);
        }

        public async Task ReceiveMessageAsync(Func<string, Task> messageHandler)
        {
            if (_channel == null)
                await InitializeConnectionAsync(); 

            //await _channel!.QueueDeclareAsync("processing-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += (model, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                messageHandler(message);
                return Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(queue: "processing-queue", autoAck: true, consumer: consumer);

            await Task.Delay(Timeout.Infinite); 
        }

        private async Task SetupQueues()
        {
            var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "delayed-exchange" },
            { "x-dead-letter-routing-key", "delayed-key" },
            { "x-message-ttl", 3600000 }
        };

            await _channel.QueueDeclareAsync("delayed-queue", durable: true, exclusive: false, autoDelete: false, arguments: arguments);
            await _channel.QueueDeclareAsync("processing-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            await _channel.ExchangeDeclareAsync("delayed-exchange", type: "direct");
            await _channel.QueueBindAsync("processing-queue", "delayed-exchange", "delayed-key");
        }


        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
        }
    }

}
