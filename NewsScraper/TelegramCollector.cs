using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;
using NewsConcentratorSystem.NewsScraper;
using TLSchema;
using TLSchema.Messages;
using TLSharp;
using Timer = System.Timers.Timer;

namespace NewsConcentratorSystem.NewsScraper
{
    public class TelegramCollector
    {
        Timer keepconnected_timer = new Timer(30000);
        public TelegramClient client;
        public NewsConcentratorDbContext _Context;
        public TelegramCollector(TelegramClient client, NewsConcentratorDbContext context)
        {
            this.client = client;
            this._Context = context;
            keepconnected_timer.Elapsed += GetNewses;
        }



        //Collector Unit...
        public async void GetNewses(Object source, System.Timers.ElapsedEventArgs e)
        {
            Program.keepconnected_timer.Stop();
            Thread.Sleep(1200);
            var dialogs = (TLDialogsSlice)await client.GetUserDialogsAsync();
            Program.keepconnected_timer.Start();
            var channels = _Context.Channels.ToList();



            foreach (var channel in channels)
            {
                var messages = TelegramClientManager.GetChannelUnreadmessages(channel.ChannelUserName, dialogs).Result;

                var photomessages  =messages.Where(m => m.Media != null && m.Media is TLMessageMediaPhoto).Select(m => m.Media).OfType<TLMessageMediaPhoto>();
                var textmessages = messages.Where(m => m.Media == null)/*.Where("")* for ensure message in not forwarded*/.Select(m => m.Message);


                #region MustCuntainFiltering

                

                //MustContainFilter
                _Context.Entry(channel).Collection(channels => channel.MustContainWords).Load();


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
}
