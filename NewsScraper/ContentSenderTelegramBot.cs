using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static string _DestinatnUsername;
        public ContentSenderTelegramBot(TelegramBotClient botClient, string Destinatn)
        {
            this._BotClient = botClient;
            _BotClient.StartReceiving();
        }


        public void SendTextMessage(string messagetext)
        {
            try
            {
                _BotClient.SendTextMessageAsync(new ChatId(-1001479837640), messagetext).Wait();
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
                _BotClient.SendPhotoAsync(new ChatId(-1001479837640), new InputOnlineFile(fileStream), caption).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
