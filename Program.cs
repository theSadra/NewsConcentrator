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
        public static TelegramCollector collector;



        public static void Main(string[] args)
        {


         

            #region TelegramClientConfiguraition




            int app_id = 4290367;
            string api_hash = "4bf7b77ff1072c71f94e3ff4a883777a";
            var store = new FileSessionStore();



            Console.WriteLine("Choose authentication method:\n");
            Console.WriteLine("1) Login by phone number authentication");
            Console.WriteLine("2) Store last session file");




            var cki = Console.ReadKey();
            if (cki.KeyChar == '2')
            {
                try
                {

                    client = new TelegramClient(app_id, api_hash, store);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Environment.Exit(-1);
                }
                TelegramClientManager._client = client;
            }

            else if(cki.KeyChar == '1')
            {
                TelegramClientManager.CreateClientbyAuthorize(app_id, api_hash).Wait();
                client = TelegramClientManager._client;
            }

 


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

            keepconnected_timer.Enabled = false;

            //var client = new TelegramClient(app_id,api_hash);

            //Console.Write("Enter numberphone:");
            //var numberphone = Console.ReadLine();

            //var hash = await client.SendCodeRequestAsync(numberphone);
            Console.WriteLine("Is connected: " + client.IsConnected);
            Console.WriteLine("Is user authorized: " + client.IsUserAuthorized());

            collector = new TelegramCollector();
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
