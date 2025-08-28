using Azure.Storage.Queues;

namespace LKcosmetics_CLDV6212_POE.Services
{
    public class QueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(string connectionString, string queueName)
        {
            _queueClient = new QueueClient(connectionString, queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task EnqueueMessageAsync(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }
    }
}
