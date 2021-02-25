using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.NewsScraper;
using TLSchema;
using TLSharp;
using NewsConcentratorSystem.Models;

namespace NewsConcentratorSystem
{
    public class Program
    {
       public static Timer keepconnected_timer = new Timer(30000);

        private static TelegramClient client = null;
        public static NewsConcentratorDbContext DbContext = new NewsConcentratorDbContext();

        static void OnkeepAlive(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //send message
                client.SendMessageAsync(new TLInputPeerUser() { UserId = 1585250390 }, "trying to keep alive...");

            }
            catch
            {
                Console.WriteLine("Err in keeping alive opration......");
                Console.ReadKey();
                //countinue
            }
        }

        public static void Main(string[] args)
        {

            #region TelegramClientConfiguraition
            Timer keepconnected_timer = new Timer(30000);
            keepconnected_timer.Elapsed += OnkeepAlive;



            int app_id = 2372991;
            string api_hash = "c7f27d96d2b3409d0b48d9682a3314a4";
            var store = new FileSessionStore();

            client = new TelegramClient(app_id, api_hash, store);

            TelegramClientManager._client = client;
            //await TelegramClientManager.CreateClientbyAuthorize(app_id, api_hash);
            //client = TelegramClientManager._client;


            while (true)
            {
                try
                {
                    //send message
                    client.ConnectAsync(false).Wait();
                    break;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("/n /n any key to retying...");
                    Console.ReadKey();
                    //countinue
                }
            }

            OnkeepAlive(null, null);
            keepconnected_timer.Enabled = true;

            //var client = new TelegramClient(app_id,api_hash);

            //Console.Write("Enter numberphone:");
            //var numberphone = Console.ReadLine();

            //var hash = await client.SendCodeRequestAsync(numberphone);
            NewsConcentratorDbContext db = new NewsConcentratorDbContext();
            Console.WriteLine("Is connected: " + client.IsConnected);
            Console.WriteLine("Is user authorized: " + client.IsUserAuthorized());
            TelegramCollector collector = new TelegramCollector(client,db);
            // Console.Write("Enter Verifacation code:");


            #endregion

            //    Console.WriteLine("hellop hellop wordpppp");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        

    }

}
