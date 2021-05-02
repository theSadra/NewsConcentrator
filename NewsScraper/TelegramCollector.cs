using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using NewsConcentratorSystem.Models;
using NewsConcentratorSystem.NewsScraper;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TLSchema;
using TLSchema.Messages;
using TLSharp;
using File = System.IO.File;
using Timer = System.Timers.Timer;

namespace NewsConcentratorSystem.NewsScraper
{
    public class TelegramCollector
    {
        public TelegramBotClient bot;
        Timer Collector = new Timer(1000);
        public static bool mustwait = false;
        public static int mustwaittilme = 0;
        public static ContentSenderTelegramBot _Bot;
        public static TelegramClient client;
        public NewsConcentratorDbContext _Context;
        Timer keepconnected_timer = new Timer(30000);
        IHasher hasher = new SHA256Hasher();

        private void OnTelegramMessage(object sender, MessageEventArgs e)
        {
            //if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && e.Message.ext == "/start")
            //    bot.SendTextMessageAsync(new ChatId(e.Message.MigrateFromChatId), "Started...").Wait();
        }

        public TelegramCollector()
        {

            bot = new TelegramBotClient("1614127935:AAGWfaa6RwOrrUGH2V0AR9phluns7ScvpFk");
            //bot.SendTextMessageAsync(new ChatId(-1001266511682), "Just setting khbrdnn.");
            var me = bot.GetMeAsync().Result;
            //bot.SendTextMessageAsync(new ChatId(-1001479837640), "Hello word.!");
            //keepconnected_timer.Elapsed += OnkeepAlive;
            _Bot = new ContentSenderTelegramBot(bot);
            bot.OnMessage += OnTelegramMessage;
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


            Console.WriteLine("Is connected: " + client.IsConnected);
            Console.WriteLine("Is user authorized: " + client.IsUserAuthorized());

            this._Context = new NewsConcentratorDbContext();
            Thread t = new Thread(GetNewses);
            t.Start();
        }





        //Collector Unit...
        public async void GetNewses()
        {
            PublishedNewsExpirer expirer = new PublishedNewsExpirer(5, _Context);




            Settings settings;
            while (true)
            {

                expirer.Expirer();


                if (mustwait)
                {
                    mustwait = false;
                    Thread.Sleep(mustwaittilme);
                }

                if (!mustwait)
                {
                    Thread.Sleep(25000);
                    //Program.keepconnected_timer.Stop();
                    var dialogs = new TLDialogs();
                    try
                    {
                        if (mustwait)
                        {
                            continue;
                        }

                        //Geting dialogs from Telegram Servers
                        dialogs = await client.GetUserDialogsAsync() as TLDialogs
                            ;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }

                    //Program.keepconnected_timer.Start();
                    settings = _Context.Settings.AsNoTracking().FirstOrDefault();

                    var channels = _Context.Channels.ToList();


                    try
                    {


                        foreach (var channel in channels)
                    {
                        var publishednewses = _Context.PublishedNewses.AsNoTracking();
                        var RepetitiousNewstextDetector = new RepetitiousNewsTextDetector(publishednewses, 47);

                        IEnumerable<TLMessage> messages = null;
                        try
                        {

                            messages = TelegramClientManager
                                .GetChannelUnreadmessages(channel.ChannelUserName, dialogs)
                                .Result;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            continue;
                        }

                        if (messages == null)
                            continue;

                        var photomessages = messages.Where(m => m.Media != null && m.Media is TLMessageMediaPhoto)
                            .Select(m => m.Media).OfType<TLMessageMediaPhoto>().ToList();
                        var textmessages = messages
                            .Where(m => m.Media == null) /*.Where("")* for ensure message in not forwarded*/
                            .Select(m => m.Message).ToList();

                        messages = null;

                        //Filters

                        _Context.Entry(channel).Collection(channel => channel.MustContainWords).Load();







                        #region MustCuntainFiltering

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

                        #region MustNotContainFilter


                        if (channel.MustnotContainFilters != null)
                        {


                            foreach (var textmessage in textmessages)
                            {

                                foreach (var mustnotcontain in channel.MustnotContainFilters)
                                {
                                    if (textmessage.Contains(mustnotcontain.MustnotContainWord))
                                    {
                                        textmessages.Remove(textmessage);
                                    }
                                }


                            }

                            foreach (var photomessage in photomessages)
                            {

                                foreach (var mustnotcontain in channel.MustnotContainFilters)
                                {
                                    if (photomessage.Caption.Contains(mustnotcontain.MustnotContainWord))
                                    {
                                        photomessages.Remove(photomessage);
                                    }
                                }


                            }
                        }

                        #endregion






                        #region Removing Link&Tag on photos






                        foreach (var photo in photomessages)
                        {

                                photo.Caption = photo.Caption.Replace("💸", "");
                                photo.Caption = photo.Caption.Replace("➖", "");
                                photo.Caption = photo.Caption.Replace("🆔", "");
                                photo.Caption = photo.Caption.Replace("👇🏼", "");
                                photo.Caption = photo.Caption = Regex.Replace(photo.Caption, "</?(a|A).*?>", "");

                            var splitedwordlist = photo.Caption.Split(' ').ToList();

                            for (int i = 0; i < splitedwordlist.Count; i++)
                            {
                                //removing instagram taglinks
                                if (splitedwordlist[i].Contains("@"))
                                {

                                    if (splitedwordlist[i].Contains("\n"))
                                    {
                                        var linksplitedwordlist = splitedwordlist[i].Split('\n').ToList();
                                        splitedwordlist[i] = null;
                                        foreach (var li in linksplitedwordlist.ToList())
                                        {
                                            if (li.Contains("@") || li == "")
                                            {
                                                linksplitedwordlist.Remove(li);
                                            }
                                        }

                                        string filteredword = null;
                                        foreach (var li in linksplitedwordlist)
                                        {
                                            filteredword += li + " ";
                                        }

                                        if (filteredword == null)
                                        {
                                            splitedwordlist.Remove(splitedwordlist[i]);
                                            continue;
                                        }

                                        splitedwordlist[i] = filteredword;
                                    }
                                    else
                                    {
                                        splitedwordlist.Remove(splitedwordlist[i]);
                                    }

                                }

                                if (!(i < splitedwordlist.Count))
                                {
                                    break;
                                }

                                if (splitedwordlist[i].Contains("://") || splitedwordlist[i].Contains(".com") ||
                                    splitedwordlist[i].Contains(".ir") || splitedwordlist[i].Contains(".org") ||
                                    splitedwordlist[i].Contains(".net") || splitedwordlist[i].Contains(".ai"))
                                {

                                    if (splitedwordlist[i].Contains("\n"))
                                    {
                                        var linksplitedwordlist = splitedwordlist[i].Split('\n').ToList();
                                        splitedwordlist[i] = null;
                                        foreach (var li in linksplitedwordlist.ToList())
                                        {
                                            if (li.Contains("://") || li.Contains(".me") || li.Contains(".ir") ||
                                                li.Contains(".com") || li.Contains(".org") || li.Contains(".net") ||
                                                li == "" || li.Contains(".ai") || li.Contains("💸") || li.Contains("👇🏼") || li.Contains("➖") || li.Contains("🆔"))
                                            {
                                                linksplitedwordlist.Remove(li);
                                            }
                                        }

                                        string filteredword = null;
                                        foreach (var li in linksplitedwordlist)
                                        {
                                            filteredword += li + " ";
                                        }

                                        if (filteredword == null)
                                        {
                                            splitedwordlist.Remove(splitedwordlist[i]);
                                            continue;
                                        }

                                        splitedwordlist[i] = filteredword;

                                    }
                                    else
                                    {
                                        var linksplitedwordlist = splitedwordlist[i].Split(' ').ToList();
                                        splitedwordlist[i] = null;
                                        foreach (var li in linksplitedwordlist.ToList())
                                        {
                                            if (li.Contains("://") || li.Contains(".me") || li.Contains(".ir") || li.Contains(".ai") || li.Contains("💸") || li.Contains("👇🏼") || li.Contains("➖") || li.Contains("🆔"))
                                            {
                                                linksplitedwordlist.Remove(li);
                                            }
                                        }

                                        string filteredword = null;
                                        foreach (var li in linksplitedwordlist)
                                        {
                                            filteredword += li + " ";
                                        }

                                        splitedwordlist[i] = filteredword;
                                    }

                                }
                                if (!(i < splitedwordlist.Count))
                                {
                                    break;
                                }
                                if (splitedwordlist[i].Contains("www."))
                                {
                                    if (!(i < splitedwordlist.Count))
                                    {
                                        break;
                                    }

                                    if (splitedwordlist[i].Contains("\n"))
                                    {
                                        var linksplitedwordlist = splitedwordlist[i].Split('\n').ToList();
                                        splitedwordlist[i] = null;
                                        foreach (var li in linksplitedwordlist.ToList())
                                        {
                                            if (!(i < splitedwordlist.Count) || li == null)
                                            {
                                                break;
                                            }
                                            if (li.Contains("www."))
                                            {
                                                linksplitedwordlist.Remove(li);
                                            }
                                        }

                                        foreach (var li in linksplitedwordlist)
                                        {
                                            splitedwordlist[i] += li + " ";
                                        }

                                    }
                                    else
                                    {
                                        splitedwordlist.Remove(splitedwordlist[i]);
                                    }

                                }
                            }

                            string filteredcaption = null;
                            foreach (var split in splitedwordlist.ToList())
                            {
                                filteredcaption += split + " ";
                            }

                            Regex.Replace(photo.Caption, "</?(a|A).*?>", "");

                            photo.Caption = filteredcaption;

                        }

                        #endregion


                        #region Removing Link&Tag on textMessages


                        for (int j = 0; j < textmessages.Count; j++)
                        {
                           textmessages[j] = Regex.Replace(textmessages[j], "</?(a|A).*?>", "");

                            var splitedwordlist = textmessages[j].Split(' ').ToList();




                            for (int i = 0; i < splitedwordlist.Count; i++)
                            {
                                if (!(i < splitedwordlist.Count))
                                {
                                    break;
                                }
                                //removing telegram taglinks
                                if (splitedwordlist[i].Contains("@"))
                                {

                                    if (splitedwordlist[i].Contains("\n"))
                                    {
                                        var linksplitedwordlist = splitedwordlist[i].Split('\n').ToList();
                                        splitedwordlist[i] = null;
                                        foreach (var li in linksplitedwordlist.ToList())
                                        {
                                            if (li.Contains("@") || li == "")
                                            {
                                                linksplitedwordlist.Remove(li);
                                            }
                                        }

                                        string filteredword = null;
                                        foreach (var li in linksplitedwordlist)
                                        {
                                            filteredword += li + " ";
                                        }

                                        if (filteredword == null)
                                        {
                                            splitedwordlist.Remove(splitedwordlist[i]);
                                            continue;
                                        }

                                        splitedwordlist[i] = filteredword;
                                    }
                                    else
                                    {
                                        splitedwordlist.Remove(splitedwordlist[i]);
                                    }

                                }

                                if (i >= splitedwordlist.Count)
                                {
                                    i--;
                                }

                                if (splitedwordlist[i].Contains("://") || splitedwordlist[i].Contains(".com") ||
                                    splitedwordlist[i].Contains(".ir") || splitedwordlist[i].Contains(".org") ||
                                    splitedwordlist[i].Contains(".net") || splitedwordlist[i].Contains(".ai") ||
                                    splitedwordlist[i].Contains("💸") || splitedwordlist[i].Contains("👇🏼") ||
                                    splitedwordlist[i].Contains("➖") || splitedwordlist[i].Contains("🆔"))
                                {

                                    if (splitedwordlist[i].Contains("\n"))
                                    {
                                        var linksplitedwordlist = splitedwordlist[i].Split('\n').ToList();
                                        splitedwordlist[i] = null;
                                        foreach (var li in linksplitedwordlist.ToList())
                                        {
                                            if (li.Contains("://") || li.Contains(".me") || li.Contains(".ir") ||
                                                li.Contains(".com") || li.Contains(".org") || li.Contains(".ai") ||
                                                li.Contains(".net") ||
                                                li == "" || li.Contains("💸") || li.Contains("👇🏼") ||
                                                li.Contains("➖") || li.Contains("🆔"))
                                            {
                                                linksplitedwordlist.Remove(li);
                                            }
                                        }

                                        string filteredword = null;
                                        foreach (var li in linksplitedwordlist)
                                        {
                                            filteredword += li + " ";
                                        }

                                        if (filteredword == null)
                                        {
                                            splitedwordlist.Remove(splitedwordlist[i]);
                                            continue;
                                        }

                                        splitedwordlist[i] = filteredword;

                                    }
                                    else
                                    {
                                        var linksplitedwordlist = splitedwordlist[i].Split(' ').ToList();
                                        splitedwordlist[i] = null;
                                        foreach (var li in linksplitedwordlist.ToList())
                                        {
                                            if (li.Contains("://") || li.Contains(".me") || li.Contains(".ir") ||
                                                li == "" || li.Contains(".ai"))
                                            {
                                                linksplitedwordlist.Remove(li);
                                            }
                                        }

                                        string filteredword = null;
                                        foreach (var li in linksplitedwordlist)
                                        {
                                            filteredword += li + " ";
                                        }

                                        splitedwordlist[i] = filteredword;
                                    }

                                }


                                if (splitedwordlist[i].Contains("www."))
                                {

                                    if (splitedwordlist[i].Contains("\n"))
                                    {
                                        var linksplitedwordlist = splitedwordlist[i].Split('\n').ToList();
                                        splitedwordlist[i] = null;
                                        foreach (var li in linksplitedwordlist.ToList())
                                        {
                                            if (li.Contains("www."))
                                            {
                                                linksplitedwordlist.Remove(li);
                                            }
                                        }

                                        foreach (var li in linksplitedwordlist)
                                        {
                                            splitedwordlist[i] += li + " ";
                                        }

                                    }
                                    else
                                    {
                                        splitedwordlist.Remove(splitedwordlist[i]);
                                    }

                                }
                            }

                            string filteredcaption = null;
                            foreach (var split in splitedwordlist.ToList())
                            {
                                filteredcaption += split + " ";
                            }


                            textmessages[j] = filteredcaption;

                        }


                        #endregion





                        _Context.Entry(channel).Collection(channel => channel.ReplaceWords).Load();


                        #region WordReplacement Filter on photos

                        foreach (var photo in photomessages)
                        {
                           photo.Caption = photo.Caption.Replace("➖", "");


                            foreach (var replaceWord in channel.ReplaceWords)
                            {
                                photo.Caption = photo.Caption.Replace(replaceWord.Word, replaceWord.ReplaceTo);
                            }




                            var splitedwordlist = photo.Caption.Split(' ').ToList();
                            for (int i = 0; i < splitedwordlist.Count; i++)
                            {
                                foreach (var replaceWord in channel.ReplaceWords)
                                {
                                    if (splitedwordlist[i] == replaceWord.Word)
                                    {
                                        splitedwordlist[i] = replaceWord.ReplaceTo;
                                    }
                                }
                            }

                            string filteredcaption = null;
                            foreach (var split in splitedwordlist.ToList())
                            {
                                filteredcaption += split + " ";
                            }


                            photo.Caption = filteredcaption;
                        }


                        #endregion


                        #region WordReplacement Filter on textMessages

                        for (int i = 0; i < textmessages.Count; i++)
                        {

                            textmessages[i] =  textmessages[i].Replace("➖", "");

                                foreach (var replaceWord in channel.ReplaceWords)
                                {
                                    textmessages[i] = textmessages[i].Replace(replaceWord.Word, replaceWord.ReplaceTo);
                                }
                            
                        }





                        for (int j = 0; j < textmessages.Count; j++)
                        {
                            var splitedwordlist = textmessages[j].Split(' ').ToList();
                            for (int i = 0; i < splitedwordlist.Count; i++)
                            {
                                foreach (var replaceWord in channel.ReplaceWords)
                                {
                                    if (splitedwordlist[i] == replaceWord.Word)
                                    {
                                        splitedwordlist[i] = replaceWord.ReplaceTo;
                                    }
                                }
                            }

                            string filteredcaption = null;
                            foreach (var split in splitedwordlist.ToList())
                            {
                                filteredcaption += split + " ";
                            }


                            textmessages[j] = filteredcaption;
                        }

                        #endregion

                        if (mustwait)
                        {
                            mustwait = false;
                            Thread.Sleep(mustwaittilme);
                        }

                        #region Sending Text Messages to the Destination

                        //for test




                        foreach (var message in textmessages)
                        {





                            var news = new News()
                            {
                                //Todo: Conjunction word must remove

                                TextMessage = message,

                                DateAdded = DateTime.Now
                            };
                            var textRepetitousresult =
                                RepetitiousNewstextDetector.IsNewstextRepetitous(news);
                            if (textRepetitousresult)
                            {
                                continue; //Avoid sending
                            }









                            _Bot.SendTextMessage(settings.StartDescription + "\n" + message + "\n" + "\n" +
                                                 settings.EndDescription);
                            //Calling marker
                            MarkasPublished(news);
                            Thread.Sleep(2560);
                        }


                        #endregion

                        if (mustwait)
                        {
                            mustwait = false;
                            Thread.Sleep(mustwaittilme);
                        }

                        #region Sending Photo Messages to the Destination




                        foreach (var photo in photomessages)
                        {
                            byte[] media = null;
                            try
                            {
                                //Downloading media content
                                media = TelegramClientManager.DownloadPhotoFile((TLPhoto) photo.Photo).Result;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                Thread.Sleep(10000);
                                continue;
                            }

                            var news = new News()
                            {
                                //Todo: Cunjunction word must remove

                                TextMessage = photo.Caption != null ? photo.Caption : null,

                                Mediahash = media != null ? hasher.Getfilehash(media) : null,

                                DateAdded = DateTime.Now
                            };

                            #region RepetituousNewsFilter

                            //Check if photo of current news already published , remove that

                            var mediaresult =
                                RepetitiousNewsMediaDetector.IsmediaRepetitious(hasher.Getfilehash(media),
                                    publishednewses);



                            if (mediaresult == true)
                            {
                                continue; //Avoid sending
                            Console.WriteLine("***********************REPE media NEWS********************");

                            }



                            //Text repetitious detector
                            var textRepetitousresult =
                                RepetitiousNewstextDetector.IsNewstextRepetitous(news);
                            if (textRepetitousresult)
                            {
                                continue; //Avoid sending
                                Console.WriteLine("***********************REPE text NEWS********************");

                            }






                            #endregion









                            Thread.Sleep(5000);
                            var mediafilestream = new MemoryStream(media);

                            //Mark as a published news


                            //Calling marker
                            MarkasPublished(news);







                            _Bot.SendPhotoMessage(mediafilestream,
                                settings.StartDescription + "\n" + photo.Caption + "\n" + "\n" +
                                settings.EndDescription);
                            Thread.Sleep(1500);
                        }

                        #endregion

                    }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

            }
        }






        static void OnkeepAlive(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //send message
                client.SendMessageAsync(new TLInputPeerUser() {UserId = 1585250390}, "trying to keep alive...");

            }
            catch
            {
                Console.WriteLine("Err in keeping alive opration......");
                Console.ReadKey();
                //countinue
            }
        }

        //Save news on PublishedNewses table
        public void MarkasPublished(News news)
        {
            _Context.PublishedNewses.Add(news);
            _Context.SaveChanges();
        }
    }
}

