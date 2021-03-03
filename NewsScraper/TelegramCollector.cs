using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;
using NewsConcentratorSystem.NewsScraper;
using Telegram.Bot;
using TLSchema;
using TLSchema.Messages;
using TLSharp;
using Timer = System.Timers.Timer;

namespace NewsConcentratorSystem.NewsScraper
{
    public class TelegramCollector
    {
        public TelegramBotClient bot;
        Timer Collector = new Timer(1000);
        public static bool mustwait = false;
        public static TelegramClient client;
        public NewsConcentratorDbContext _Context;
        Timer keepconnected_timer = new Timer(30000);
        public TelegramCollector()
        {
            bot = new TelegramBotClient("1614127935:AAEqBC-r7sxG3tcOhWVkjOiiq7_hHAAWR40");
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

            this._Context = new NewsConcentratorDbContext();
            Thread t = new Thread(GetNewses);
            t.Start();
        }



        //Collector Unit...
        public async void GetNewses()
        {
            while (true)
            {
                if (mustwait)
                {
                    Thread.Sleep(7500);
                    mustwait = false;
                }
                keepconnected_timer.Stop();
                Thread.Sleep(3000);
                

                //Program.keepconnected_timer.Stop();
                var dialogs = (TLDialogsSlice)await client.GetUserDialogsAsync();
                //Program.keepconnected_timer.Start();
                keepconnected_timer.Start();

                var channels = _Context.Channels.ToList();



                foreach (var channel in channels)
                {
                    var messages = TelegramClientManager.GetChannelUnreadmessages(channel.ChannelUserName, dialogs)
                        .Result;

                    if (messages == null)
                        continue;

                    var photomessages = messages.Where(m => m.Media != null && m.Media is TLMessageMediaPhoto)
                        .Select(m => m.Media).OfType<TLMessageMediaPhoto>();
                    var textmessages = messages
                        .Where(m => m.Media == null) /*.Where("")* for ensure message in not forwarded*/
                        .Select(m => m.Message);


                    #region MustCuntainFiltering



                    //MustContainFilter
                    _Context.Entry(channel).Collection(channel => channel.MustContainWords).Load();


                    if (channel.MustContainWords.Count > 0)
                    {
                        List<string> filtered_textmessages = new List<string>();
                        foreach (var textmmesage in textmessages)
                        {
                            foreach (var word in channel.MustContainWords)
                            {
                                if (textmmesage.Contains(word.MustContainWord))
                                {
                                    filtered_textmessages.Add(textmmesage);
                                    break;
                                }
                            }
                        }

                        textmessages = filtered_textmessages.Distinct().ToList();

                        List<TLMessageMediaPhoto> filtered_photomessages = new List<TLMessageMediaPhoto>();
                        foreach (var photomessage in photomessages)
                        {
                            //var photoMedia = (TLMessageMediaPhoto)photomessage.Media;
                            foreach (var word in channel.MustContainWords)
                            {
                                if (photomessage.Caption.Contains(word.MustContainWord))
                                {
                                    filtered_photomessages.Add(photomessage);
                                    break;
                                }
                            }
                        }

                        photomessages = filtered_photomessages.Distinct().ToList();




                    }

                    #endregion



                }


            }
        }






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


    }
}
