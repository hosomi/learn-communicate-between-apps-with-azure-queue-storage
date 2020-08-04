using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace learn_queue_app
{
    class Program
    {
        private const string ConnectionString = "";
        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                string value = String.Join(" ", args);
                await SendArticleAsync(value);
                Console.WriteLine($"Sent: {value}");
            }
            else
            {
                string value = await ReceiveArticleAsync();
                Console.WriteLine($"Received {value}");
            }
        }
        static async Task SendArticleAsync(string newsMessage)
        {
            CloudQueue queue = GetQueue();
            if (await queue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("The queue of news articles was created.");
            }

            CloudQueueMessage articleMessage = new CloudQueueMessage(newsMessage);
            await queue.AddMessageAsync(articleMessage);
        }

        static CloudQueue GetQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference("newsqueue");
        }

        static async Task<string> ReceiveArticleAsync()
        {
            CloudQueue queue = GetQueue();
            bool exists = await queue.CreateIfNotExistsAsync();
            if (!exists)
            {
                CloudQueueMessage retrievedArticle = await queue.GetMessageAsync();
                if (retrievedArticle != null)
                {
                    string newsMessage = retrievedArticle.AsString;
                    await queue.DeleteMessageAsync(retrievedArticle);
                    return newsMessage;
                }
            }
            return "<queue empty or not created>";
        }
    }
}
