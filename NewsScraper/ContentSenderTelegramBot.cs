using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace NewsConcentratorSystem.NewsScraper
{
    public class ContentSenderTelegramBot
    {
        public TelegramBotClient _BotClient;
        public ContentSenderTelegramBot(TelegramBotClient botClient)
        {
            this._BotClient = botClient;
            _BotClient.StartReceiving();
        }


        public void SendTextMessage(string messagetext)
        {
            try
            {
                _BotClient.SendTextMessageAsync(new ChatId(-1001266511682), messagetext + "\n\n" + "@Khabardun1").Wait();
                Thread.Sleep(1250);
                _BotClient.SendTextMessageAsync(new ChatId(-1001230277569), messagetext + "\n\n" + "@AFLAKIUN1").Wait();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SendPhotoMessage(Stream fileStream, string caption)
        {
            try
            {
                _BotClient.SendPhotoAsync(new ChatId(-1001266511682), new InputOnlineFile(fileStream), caption + "\n\n" + "@Khabardun1").Wait();
                Thread.Sleep(1250);
                _BotClient.SendPhotoAsync(new ChatId(-1001230277569), new InputOnlineFile(fileStream), caption + "\n\n" + "@AFLAKIUN1").Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
