using System;
using System.IO;
using System.Threading;
using DlistTop;
using Microsoft.Extensions.Configuration;

namespace Example
{
    class Program
    {
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        private static IConfiguration _config;

        static void Main(string[] args)
        {
            LoadConfig();
            var token = _config.GetValue<string>("token");
            var client = new DlistClient(token);
            
            client.OnReady += (sender, args) =>
            {
                Console.WriteLine($"ready! connected to {args.Data.Name}");
            };

            client.OnVote += (sender, args) =>
            {
                Console.WriteLine($"{args.Data.AuthorID} voted!");
            };
            
            client.OnRate += (sender, args) =>
            {
                Console.WriteLine($"{args.Data.AuthorID} rated - {args.Data.Rating} stars!");
            };
            
            client.Connect().Wait();

            ExitEvent.WaitOne();
        }

        static void LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Join(Environment.CurrentDirectory, "../../../"))
                .AddJsonFile("config.json", optional: false);

            _config = builder.Build();
        }
    }
}
