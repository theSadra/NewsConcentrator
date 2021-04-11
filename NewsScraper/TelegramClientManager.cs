using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TLSchema;
using TLSchema.Account;
using TLSchema.Messages;
using TLSharp;
using TLSharp.Utils;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Schema;
using TLSchema.Channels;
using TLRequestReadHistory = TLSchema.Channels.TLRequestReadHistory;

namespace NewsConcentratorSystem.NewsScraper
{
    static class TelegramClientManager
    {
        static public TelegramClient _client;

        static Random rnd = new Random();
        static int i_Loginreplaytime = 1;


        //client Account Authorization:


        public static async Task Start(int appid, string apihash,bool directauthorize = false)
        {
            await CreateClientbyLogIn(appid, apihash);

            while (true)
            {


                try
                {
                    if (directauthorize)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "/session.dat"))
                        {
                            //Deleting session file...
                            File.Delete(Environment.CurrentDirectory + "/session.dat");
                        }

                        
                        Console.WriteLine(" Trying Authorization with verification code.......");
                        //Calling CreateClientbyAuthorize method.. switch to authorization method 
                        await CreateClientbyAuthorize(appid, apihash);
                        return;
                    }

                    //trying logging in...
                    Console.WriteLine("Trying logging in...");


                    if (_client.IsUserAuthorized())
                    {

                        Console.Write("Logging in to account was");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" successfully");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" and client.authorized is {_client.IsUserAuthorized()}.");

                    }
                    //trying authorization...
                    else
                    {
                        if (File.Exists(Environment.CurrentDirectory + "/session.dat"))
                        {
                            //Deleting session file...
                            File.Delete(Environment.CurrentDirectory + "/session.dat");
                        }

                        Console.Write("Login Authorization");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" failed");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(" Trying Authorization with verification code.......");
                        //Calling CreateClientbyAuthorize method.. switch to authorization method 
                        await CreateClientbyAuthorize(appid, apihash);
                    }
                    break;
                }
                catch (Exception e)
                {
                    if (i_Loginreplaytime <= 3)
                    {
                        Console.WriteLine(e.Message + " Happen...\n");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"retrying for {i_Loginreplaytime}/3 ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    //else: Exit from start and back to method caller...
                }
            }
        }

        public static async Task CreateClientbyAuthorize(int api_id, string api_hash, string numberphone = null)
        {
            _client = new TelegramClient(api_id, api_hash, null, Environment.CurrentDirectory + "/session");
            await _client.ConnectAsync(false);
            string fullPathToDat = Directory.GetCurrentDirectory() + @"\session";
            var store = new FileSessionStore();


            //getting numberphone from console
            if (numberphone == null)
                numberphone = Getphonenumberfrom_console();

            var hash = _client.SendCodeRequestAsync(numberphone).Result;
            //Getting verification code
            Console.WriteLine("Enter verification code:");
            var code = Console.ReadLine();

            //Make authorize
            await _client.MakeAuthAsync(numberphone, hash, code);

        }

        public static async Task CreateClientbyLogIn(int api_id, string api_hash)
        {
            string fullPathToDat = Directory.GetCurrentDirectory() + "\\session";
            var store = new FileSessionStore();
            _client = new TelegramClient(api_id, api_hash, store, fullPathToDat);
            await _client.ConnectAsync(true);
        }


        

        public static async Task<bool> JoinChannel(string channelusername)
        {
            //finding channel
            var update = await _client.SearchUserAsync(channelusername);
            var channel = update.Chats.Where(c => c.GetType() == typeof(TLChannel)).Cast<TLChannel>().FirstOrDefault();

            if (channel != null)
            {
                var request = new TLRequestJoinChannel()
                { Channel = new TLInputChannel() { AccessHash = (long)channel.AccessHash, ChannelId = channel.Id } };

                TLUpdates resJoinChannel = await _client.SendRequestAsync<TLUpdates>(request);
                if (resJoinChannel != null)
                {
                    Console.WriteLine($"------*successfully Joined to {channel.Title}.  ");
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }


        public static TLChannel GetTChannelbyUsername(string channelUsername)
        {
            var found =  _client.SearchUserAsync(channelUsername).Result;
            return found.Chats.Where(c => c.GetType() == typeof(TLChannel)).Cast<TLChannel>().Where(c => c.Username.ToString().ToLower() == channelUsername.ToLower()).FirstOrDefault();
        }

        public static async Task<IEnumerable<TLMessage>> GetChannelUnreadmessages(string channelUsername, TLDialogsSlice dialogs)
        {



            IEnumerable<TLMessage> unreadMessages ;

            foreach (TLDialog dialog in dialogs.Dialogs.Where(lambdaDialog => lambdaDialog.Peer is TLPeerChannel/* && lambdaDialog.UnreadCount > 0*/))
            {
                TLPeerChannel peer = (TLPeerChannel)dialog.Peer;
                TLChannel channel = dialogs.Chats.OfType<TLChannel>().First(lambdaChannel => lambdaChannel.Id == peer.ChannelId);
                Console.WriteLine(channel.Username);


                if (channel.Username == channelUsername && dialog.UnreadCount > 0)
                {
                    var target = new TLInputPeerChannel
                        { ChannelId = channel.Id, AccessHash = channel.AccessHash ?? 0 };


                    TLChannelMessages hist =
                        (TLChannelMessages)_client.GetHistoryAsync(target, 0, 0, 0).Result;


                    int takecount = 25;
                    if (dialog.UnreadCount <25)
                    {
                        takecount = dialog.UnreadCount;
                    }

                    var hists = hist.Messages.OfType<TLMessage>().ToList();
                    unreadMessages = hist.Messages
                    .Take(takecount).OfType<TLMessage>();


                    //MarkMessage as unread
                    MarkMessagesasRead(channel, unreadMessages.ToList()[0]).Wait();
                    return unreadMessages;
                }

            }

            return null;
        }

        public static async Task MarkMessagesasRead(TLChannel channel, TLMessage firstmessage)
        {

            var ch = new TLInputChannel() { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash };



            try
            {
                int mID = firstmessage.Id;
                string TMessage = firstmessage.Message;
                string TvisitCount = firstmessage.Views.ToString();

                var markAsRead = new TLRequestReadHistory()
                {
                    Channel = ch,
                    // MaxId = -1,
                    MessageId = mID,
                    // Dirty = true,
                    //  MessageId = 356217,
                    // ConfirmReceived = true,
                    //Sequence = dia.unread_count
                };
                var affectedMessages = await _client.SendRequestAsync<bool>(markAsRead);
                Console.WriteLine(mID + ' ' + TMessage + ' ' + TvisitCount + ' ' + affectedMessages);


            }
            catch { }

        }


        //Downloading actions:
        public static async Task<byte[]> DownloadPhotoFile(TLPhoto photo)
        {
            var photoSize = photo.Sizes.ToList().OfType<TLPhotoSize>().Last();
            TLFileLocation tf = (TLFileLocation)photoSize.Location;

            return _client.GetFile(new TLInputFileLocation
            {

                LocalId = tf.LocalId,
                Secret = tf.Secret,
                VolumeId = tf.VolumeId
            }
                ,
                (int)Math.Pow(2, Math.Ceiling(Math.Log(photoSize.Size, 2)))).Result.Bytes;
        }


        //Sending actions:


        public static async Task SendtextMessage()
        {

        }

        public static async Task SendImageMessagetochannel(byte[] fileBytes, string filename,string captionText, TLInputPeerChannel channelPeer)
        {


            var streamReader = new StreamReader(new MemoryStream(fileBytes));
            TLAbsInputFile file;

            //Uploading photoFile to telegram server
            try
            {
               file = (TLInputFile) await _client.UploadFile(filename, streamReader);
            }
            catch
            {
                throw new Exception("err uploading file");
            }

            //Sending file to channel
            try
            {
                await _client.SendUploadedPhoto(channelPeer, file, captionText);
            }
            catch (Exception e)
            {
                throw  new Exception(e.Message);
            }


        }

        //public static async Task<byte[]> DownloadVideoFile(TLDocument document)
        //{

        //    //if (message.Media == null)
        //    //    return null;


        //    //var document = (TLDocument)((TLMessageMediaDocument)message.Media).Document;

        //    //var document = hist.Messages
        //    //        .OfType<TLMessage>()
        //    //        .Where(m => m.Media != null)
        //    //        .Select(m => m.Media)
        //    //        .OfType<TLMessageMediaDocument>()
        //    //        .Select(md => md.Document)
        //    //        .OfType<TLDocument>()
        //    //        .First();



        //    var resFile = await _client.GetFile(
        //        new TLInputDocumentFileLocation()
        //        {
        //            AccessHash = document.AccessHash,
        //            Id = document.Id,
        //            Version = document.Version
        //        },
        //        (int)Math.Pow(2, Math.Ceiling(Math.Log(document.Size, 2))));

        //    return resFile.Bytes;

        //}





        //var users = hist.Users.OfType<TLUser>();
        //var messages = hist.Messages.OfType<TLMessage>().ToList();

        //public static async Task/*<List<TLObject>>*/ GetallUnreadmessages()
        //{

        //    IEnumerable<TLObject> unreadmessages = null;


        //    var dialogs = (TLDialogsSlice)_client.GetUserDialogsAsync().Result;


        //    foreach (TLDialog dialog in dialogs.Dialogs.Where(lambdaDialog => lambdaDialog.Peer is TLPeerChannel && lambdaDialog.UnreadCount > 0))
        //    {
        //        TLPeerChannel peer = (TLPeerChannel)dialog.Peer;
        //        TLChannel channel = dialogs.Chats.OfType<TLChannel>().First(lambdaChannel => lambdaChannel.Id == peer.ChannelId);
        //        Console.WriteLine(channel.Title);
        //        TLInputPeerChannel target = new TLInputPeerChannel { ChannelId = channel.Id, AccessHash = channel.AccessHash ?? 0 };


        //        TLChannelMessages hist = (TLChannelMessages)_client.GetHistoryAsync(target, 0, -1, dialog.UnreadCount).Result;
        //        var users = hist.Users.OfType<TLUser>();
        //        var messages = hist.Messages.OfType<TLMessage>().ToList();

        //        var documents = hist.Messages
        //            .OfType<TLMessage>()
        //            .Where(m => m.Media != null)
        //            .Select(m => m.Media)
        //            .OfType<TLMessageMediaDocument>()
        //            .Select(md => md.Document)
        //            .OfType<TLDocument>()
        //            .ToList();



        //        foreach (var message in messages)
        //        {


        //            //Text messages
        //            if (message.Media == null)
        //            {

        //            }

        //            //PhotoMessages
        //            else if (message.Media.GetType().Name == "TLMessageMediaPhoto")
        //            {
        //                //higher quality photo:
        //                var photo = ((TLMessageMediaPhoto)message.Media).Photo as TLPhoto;
        //                //TLFileLocation location = (TLFileLocation)fullphotosize.Location;
        //                var szie = ((TLPhotoSize)photo.Sizes.Last());
        //                TLPhoto b = new TLPhoto();
        //                var c = new TLInputPhoto();



        //                var fileResult = (TLInputFile)await _client.UploadFile("cat.jpg", new StreamReader("./cat.jpg"));



        //                var size = ((TLPhotoSize)photo.Sizes.Last()).Size;
        //                var file = await _client.GetFile(
        //                       new TLInputDocumentFileLocation()
        //                       {
        //                           AccessHash = photo.AccessHash,
        //                           Id = photo.Id,
        //                       }, (int)Math.Pow(2, Math.Ceiling(Math.Log(size, 2)))); //size of fileChunk you want to retrieve






        //            }


        //            //Video messages
        //            else
        //            {


        //                //bool isvideo = false;
        //                //TLMessageMediaDocument media = (TLMessageMediaDocument) message.Media;
        //                //TLDocument doc = (TLDocument) media.Document;
        //                //foreach (var att in doc.Attributes.ToList())
        //                //{
        //                //    //video message
        //                //    if (att.GetType().Name == "TLDocumentAttributeVideo")
        //                //    {
        //                //        Console.WriteLine("Video file");
        //                //        isvideo = true;
        //                //        break;
        //                //    }
        //                //}

        //                ////Video messages
        //                //if (isvideo)
        //                //{

        //                //}
        //            }

        //        }



        //        //_client.SendUploadedPhoto(target,)

        //        //foreach (TLMessage message in messages)
        //        //{
        //        //    //TLUser sentUser = users.Single(lambdaUser => lambdaUser.Id == message.FromId);
        //        //    //////Seems forwarded from somewhere....!
        //        //    ////if (message.FwdFrom!=null)
        //        //    ////{ 
        //        //    ////    messages.Remove(message);
        //        //    ////    countine;

        //        //    ////}
        //        //    //////else:...........
        //        //    /////
        //        //    ///// 




        //        //    //Console.WriteLine($"{channel.Title} {sentUser.FirstName} {sentUser.LastName} {sentUser.Username}: {message.Message}");
        //        //}
        //    }
        //}


        //public static async Task<TLAbsUpdates> SendMessage(TLDialogsSlice dialogs,int channelid, TLInputPeerUser peer, string message = null)
        //{
        //    //get user chats 
        //    var chats = dialogs.Chats;
        //    TLInputFileLocation a = new TLInputFileLocation()
        //        {

        //        }
        //        ;


        //    //find channel by title
        //    var tlChannel = chats.Where(c=> c.GetType() == typeof(TLChannel))
        //        .Select(c => (TLChannel)c)
        //        .Where(c => c.Id==channelid)
        //        .Single();
        //    //send message
        //    await _client.SendMessageAsync(new()
        //            { ChannelId = tlChannel.Id, AccessHash = (long)tlChannel.AccessHash },
        //        "OUR_MESSAGE");
        //}









        #region Private memmbers

        private static string Getphonenumberfrom_console()
        {
            Console.WriteLine("Enter numberphone to authorize:");
            string numberphone = Console.ReadLine();
            return numberphone;
        }

        //private static string Getphonenumberfrom_file()
        //{

        //}
        private static void Disposeclient()
        {
            TelegramClientManager._client.Dispose();
        }

        #endregion
    }
}
