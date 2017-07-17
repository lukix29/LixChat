//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.WebSockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace LX29_Twitch.Api
//{
//    public enum PubSubRequestType
//    {
//        /// <summary>Type of request to listen to a specific topic</summary>
//        ListenToTopic
//    }

//    public class JSON_Builder
//    {
//        public static string Create(string topic, string nonce, string type, string token)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.AppendLine("{");
//            sb.AppendLine("\"type\": \"" + type + "\",");
//            sb.AppendLine("\"nonce\": \"" + nonce + "\",");
//            sb.AppendLine("\"data\": {");
//            sb.AppendLine("\"topics\": [\"" + topic + "\"],");
//            sb.AppendLine("\"auth_token\": \"" + token + "\"");
//            sb.AppendLine("}");
//            sb.AppendLine("}");
//            return sb.ToString();
//        }

//        public static string Create(string typeName, string value)
//        {
//            return "{\n\"" + typeName + "\":  \"" + value + "\"\n}";
//        }
//    }

//    public class Message
//    {
//        public MessageData messageData;

//        public Message(string jsonStr)
//        {
//            JToken json = JObject.Parse(jsonStr).SelectToken("data");
//            Topic = json.SelectToken("topic").ToString();
//            var encodedJsonMessage = json.SelectToken("message").ToString();
//            switch (Topic.Split('.')[0])
//            {
//                //case "chat_moderator_actions":
//                //    messageData = new ChatModeratorActions(encodedJsonMessage);
//                //    break;
//                //case "channel-bitsevents":
//                //    messageData = new ChannelBitsEvents(encodedJsonMessage);
//                //    break;
//                //case "video-playback":
//                //    messageData = new VideoPlayback(encodedJsonMessage);
//                //    break;
//                case "whispers":
//                    messageData = new Whisper(encodedJsonMessage);
//                    break;
//            }
//        }

//        public string Topic { get; protected set; }
//    }

//    public abstract class MessageData
//    {
//        //Leave empty for now
//    }

//    public class OnListenResponseArgs
//    {
//        /// <summary>Property representing the response as Response object</summary>
//        public Response Response;

//        /// <summary>Property representing if request was successful.</summary>
//        public bool Successful;

//        /// <summary>Property representing the topic that was listened to</summary>
//        public string Topic;
//    }

//    public class OnWhisperArgs
//    {
//        /// <summary>Property representing the whisper object.</summary>
//        public Whisper Whisper;

//        /// <summary>
//        ///  Whisper args class constructor.
//        /// </summary>
//        public OnWhisperArgs()
//        {
//        }
//    }

//    public class PreviousRequest
//    {
//        public PreviousRequest(string nonce, PubSubRequestType requestType, string topic = "none set")
//        {
//            Nonce = nonce;
//            RequestType = requestType;
//            Topic = topic;
//        }

//        public string Nonce { get; protected set; }
//        public PubSubRequestType RequestType { get; protected set; }
//        public string Topic { get; protected set; }
//    }

//    public class Response
//    {
//        //{"type":"RESPONSE","error":"","nonce":"8SYYENPH"}

//        public Response(string json)
//        {
//            Error = JObject.Parse(json).SelectToken("error").ToString();
//            Nonce = JObject.Parse(json).SelectToken("nonce").ToString();
//            if (string.IsNullOrWhiteSpace(Error))
//                Successful = true;
//        }

//        public string Error { get; protected set; }
//        public string Nonce { get; protected set; }
//        public bool Successful { get; protected set; }
//    }

//    /// <summary>
//    /// Class represneting interactions with the Twitch PubSub
//    /// </summary>
//    public class TwitchPubSub
//    {
//        private static Random random = new Random();
//        private bool logging;
//        private System.Timers.Timer pingTimer = new System.Timers.Timer();
//        private PreviousRequest previousRequest;
//        private WebSocketClient socket;
//        /*
//        NON-IMPLEMENTED AVAILABLE TOPICS (i'm aware of):
//        whispers.account_name - Requires OAUTH
//        video-playback.channelid
//        */

//        #region Events

//        ///// <summary>EventHandler for named event.</summary>
//        public ListenResponse OnListenResponse;

//        public PubSubServiceClosed OnPubSubServiceClosed;

//        /// <summary>EventHandler for named event.</summary>
//        public PubSubServiceConnected OnPubSubServiceConnected;

//        public delegate void ListenResponse(Response Response, string Topic, bool Successful);

//        ///// <summary>EventHandler for named event.</summary>
//        //public EventHandler<OnPubSubServiceErrorArgs> OnPubSubServiceError;
//        /// <summary>EventHandler for named event.</summary>
//        public delegate void PubSubServiceClosed();

//        public delegate void PubSubServiceConnected();

//        public delegate void WhisperEvent(Whisper whisper);

//        public event WhisperEvent OnWhisper;

//        #endregion Events

//        /// <summary>
//        /// Constructor for a client that interface's with Twitch's new PubSub system.
//        /// </summary>
//        /// <param name="_logging">Set this true to have raw messages from PubSub system printed to console.</param>
//        public TwitchPubSub(bool _logging = false)
//        {
//            logging = _logging;
//        }

//        /// <summary>
//        /// Method to connect to Twitch's PubSub service. You MUST listen toOnConnected event and listen to a Topic within 15 seconds of connecting (or be disconnected)
//        /// </summary>
//        public void Connect()
//        {
//            socket = WebSocketClient.Create(new Uri("wss://pubsub-edge.twitch.tv"));
//            socket.OnConnected += Socket_OnConnected;
//            socket.OnError += OnError;
//            socket.OnMessage += OnMessage;
//            socket.OnDisconnected += Socket_OnDisconnected;
//            socket.Connect();
//        }

//        /// <summary>
//        /// What do you think it does? :)
//        /// </summary>
//        public void Disconnect()
//        {
//            socket.Disconnect();
//        }

//        private string generateNonce()
//        {
//            return new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
//                .Select(s => s[random.Next(s.Length)]).ToArray());
//        }

//        private void listenToTopic(string topic, string oauth, bool unlisten = false)
//        {
//            string nonce = generateNonce();
//            string type = (!unlisten ? "LISTEN" : "UNLISTEN");
//            string data = JSON_Builder.Create(topic, nonce, type, oauth);
//            previousRequest = new PreviousRequest(nonce, PubSubRequestType.ListenToTopic, topic);
//            socket.SendMessage(data);
//        }

//        private void OnError(object sender, Exception e)
//        {
//            if (logging)
//                Console.WriteLine(@"[TwitchPubSub]OnError: {e.Message}");
//            //OnPubSubServiceError.Invoke(this, new OnPubSubServiceErrorArgs { Exception = e });
//        }

//        private void OnMessage(object sender, string msg)
//        {
//            if (logging)
//                Console.WriteLine(@"[TwitchPubSub] {msg}");
//            parseMessage(msg);
//        }

//        private void parseMessage(string message)
//        {
//            string type = JObject.Parse(message).SelectToken("type").ToString();

//            switch (type.ToLower())
//            {
//                case "response":
//                    Response resp = new Response(message);
//                    if (previousRequest != null && previousRequest.Nonce.ToLower() == resp.Nonce.ToLower())
//                    {
//                        if (OnListenResponse != null)
//                            OnListenResponse(resp, previousRequest.Topic, resp.Successful);
//                        return;
//                    }
//                    break;

//                case "message":
//                    Message msg = new Message(message);
//                    switch (msg.Topic.Split('.')[0])
//                    {
//                        case "whispers":
//                            Whisper whisper = (Whisper)msg.messageData;
//                            if (OnWhisper != null)
//                                OnWhisper(whisper);
//                            return;
//                        //            case "chat_moderator_actions":
//                        //                ChatModeratorActions cMA = (ChatModeratorActions)msg.messageData;
//                        //                string reason = "";
//                        //                switch (cMA.ModerationAction.ToLower())
//                        //                {
//                        //                    case "timeout":
//                        //                        if (cMA.Args.Count > 2)
//                        //                            reason = cMA.Args[2];
//                        //                       OnTimeout.Invoke(this, new OnTimeoutArgs { TimedoutBy = cMA.CreatedBy, TimedoutUser = cMA.Args[0],
//                        //                            TimeoutDuration = TimeSpan.FromSeconds(int.Parse(cMA.Args[1])), TimeoutReason = reason });
//                        //                        return;
//                        //                    case "ban":
//                        //                        if (cMA.Args.Count > 1)
//                        //                            reason = cMA.Args[1];
//                        //                       OnBan.Invoke(this, new OnBanArgs { BannedBy = cMA.CreatedBy, BannedUser = cMA.Args[0], BanReason = reason });
//                        //                        return;
//                        //                    case "unban":
//                        //                       OnUnban.Invoke(this, new OnUnbanArgs { UnbannedBy = cMA.CreatedBy, UnbannedUser = cMA.Args[0] });
//                        //                        return;
//                        //                    case "untimeout":
//                        //                       OnUntimeout.Invoke(this, new OnUntimeoutArgs { UntimeoutedBy = cMA.CreatedBy, UntimeoutedUser = cMA.Args[0] });
//                        //                        return;
//                        //                    case "host":
//                        //                       OnHost.Invoke(this, new OnHostArgs { HostedChannel = cMA.Args[0], Moderator = cMA.CreatedBy });
//                        //                        return;
//                        //                    case "subscribers":
//                        //                       OnSubscribersOnly.Invoke(this, new OnSubscribersOnlyArgs { Moderator = cMA.CreatedBy });
//                        //                        return;
//                        //                    case "subscribersoff":
//                        //                       OnSubscribersOnlyOff.Invoke(this, new OnSubscribersOnlyOffArgs { Moderator = cMA.CreatedBy });
//                        //                        return;
//                        //                    case "clear":
//                        //                       OnClear.Invoke(this, new OnClearArgs { Moderator = cMA.CreatedBy });
//                        //                        return;
//                        //                    case "emoteonly":
//                        //                       OnEmoteOnly.Invoke(this, new OnEmoteOnlyArgs { Moderator = cMA.CreatedBy });
//                        //                        return;
//                        //                    case "emoteonlyoff":
//                        //                       OnEmoteOnlyOff.Invoke(this, new OnEmoteOnlyOffArgs { Moderator = cMA.CreatedBy });
//                        //                        return;
//                        //                    case "r9kbeta":
//                        //                       OnR9kBeta.Invoke(this, new OnR9kBetaArgs { Moderator = cMA.CreatedBy });
//                        //                        return;
//                        //                    case "r9kbetaoff":
//                        //                       OnR9kBetaOff.Invoke(this, new OnR9kBetaOffArgs { Moderator = cMA.CreatedBy });
//                        //                        return;

//                        //                }
//                        //                break;
//                        //            case "channel-bitsevents":
//                        //                ChannelBitsEvents cBE = (ChannelBitsEvents)msg.messageData;
//                        //                OnBitsReceived.Invoke(this, new OnBitsReceivedArgs { BitsUsed = cBE.BitsUsed, ChannelId = cBE.ChannelId, ChannelName = cBE.ChannelName,
//                        //                    ChatMessage = cBE.ChatMessage, Context = cBE.Context, Time = cBE.Time, TotalBitsUsed = cBE.TotalBitsUsed, UserId = cBE.UserId, Username = cBE.Username});
//                        //                return;
//                        //            case "video-playback":
//                        //                VideoPlayback vP = (VideoPlayback)msg.messageData;
//                        //                switch(vP.Type)
//                        //                {
//                        //                    case VideoPlayback.TypeEnum.StreamDown:
//                        //                       OnStreamDown.Invoke(this, new OnStreamDownArgs { PlayDelay = vP.PlayDelay, ServerTime = vP.ServerTime });
//                        //                        return;
//                        //                    case VideoPlayback.TypeEnum.StreamUp:
//                        //                       OnStreamUp.Invoke(this, new OnStreamUpArgs { PlayDelay = vP.PlayDelay, ServerTime = vP.ServerTime });
//                        //                        return;
//                        //                    case VideoPlayback.TypeEnum.ViewCount:
//                        //                       OnViewCount.Invoke(this, new OnViewCountArgs { ServerTime = vP.ServerTime, Viewers = vP.Viewers });
//                        //                        return;
//                        //                }
//                        //                break;
//                    }
//                    break;
//            }
//            //if (logging)
//            //    unaccountedFor(message);
//        }

//        private void pingTimerTick(object sender, System.Timers.ElapsedEventArgs e)
//        {
//            //JObject data = new JObject(
//            //    new JProperty("type", "PING")
//            //);
//            string data = JSON_Builder.Create("type", "PING");
//            socket.SendMessage(data.ToString());
//        }

//        private void Socket_OnConnected(WebSocketClient client)
//        {
//            if (logging)
//                Console.WriteLine(@"[TwitchPubSub]OnOpen!");
//            pingTimer.Interval = 180000;
//            pingTimer.Elapsed += pingTimerTick;
//            pingTimer.Start();
//            if (OnPubSubServiceConnected != null)
//                OnPubSubServiceConnected();
//        }

//        private void Socket_OnDisconnected(WebSocketClient client)
//        {
//            if (logging)
//                Console.WriteLine(@"[TwitchPubSub]OnClose");
//            if (OnPubSubServiceClosed != null)
//                OnPubSubServiceClosed();
//        }

//        #region Listeners

//        ///// <summary>
//        ///// Sends a request to listenOn timeouts and bans in a specific channel
//        ///// </summary>
//        ///// <param name="myTwitchId">A moderator's twitch acount's ID (can be fetched from TwitchApi)</param>
//        ///// <param name="channelTwitchId">Channel ID who has previous parameter's moderator (can be fetched from TwitchApi)</param>
//        ///// <param name="moderatorOAuth">Moderator OAuth key (can be OAuth key with any scope)</param>
//        //public void ListenToChatModeratorActions(int myTwitchId, int channelTwitchId, string moderatorOAuth)
//        //{
//        //    listenToTopic(@"chat_moderator_actions.{myTwitchId}.{channelTwitchId}", moderatorOAuth);
//        //}

//        ///// <summary>
//        ///// Sends request to listenOn bits events in specific channel
//        ///// </summary>
//        ///// <param name="channelTwitchId">Channel Id of channel to listen to bitsOn (can be fetched from TwitchApi)</param>
//        ///// <param name="channelOAuth">OAuth token linked to the channel.</param>
//        //public void ListenToBitsEvents(int channelTwitchId, string channelOAuth)
//        //{
//        //    listenToTopic(@"channel-bitsevents.{channelTwitchId}", channelOAuth);
//        //}

//        /////// <summary>
//        /////// Sends request to listenOn video playback events in specific channel
//        /////// </summary>
//        /////// <param name="channelTwitchId">Channel Id of channel to listen to playback events in.</param>
//        ////public void ListenToVideoPlayback(int channelTwitchId)
//        ////{
//        ////    listenToTopic(@"video-playback.{channelTwitchId}");
//        ////}

//        /// <summary>
//        /// Sends request to listen to whispers from specific channel.
//        /// </summary>
//        /// <param name="channelTwitchId">Channel to listen to whispers on.</param>
//        /// <param name="channelOAuth">OAuth token to verify identity.</param>
//        public void ListenToWhispers(string channelTwitchId, string channelOAuth)
//        {
//            listenToTopic(@"whispers.{" + channelTwitchId + "}", channelOAuth);
//        }

//        #endregion Listeners

//        ///// <summary>
//        ///// This method will send passed json text to the message parser in order to allow forOn-demand parser testing.
//        ///// </summary>
//        ///// <param name="testJsonString"></param>
//        //public void TestMessageParser(string testJsonString)
//        //{
//        //    parseMessage(testJsonString);
//        //}
//    }

//    public class WebSocketClient
//    {
//        public readonly ClientWebSocket Client;
//        private const int ReceiveChunkSize = 1024;
//        private const int SendChunkSize = 1024;
//        private readonly CancellationToken _cancellationToken;
//        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
//        private readonly Uri _uri;

//        protected WebSocketClient(Uri uri)
//        {
//            Client = new ClientWebSocket();
//            Client.Options.KeepAliveInterval = TimeSpan.FromSeconds(60);
//            _uri = uri;
//            _cancellationToken = _cancellationTokenSource.Token;
//        }

//        public event Action<WebSocketClient> OnConnected;

//        public event Action<WebSocketClient> OnDisconnected;

//        public event Action<WebSocketClient, Exception> OnError;

//        public event Action<WebSocketClient, string> OnMessage;

//        public bool AutoReconnect { get; set; }
//        public bool IsConnected { get { return Client.State == WebSocketState.Open ? true : false; } }

//        /// <summary>
//        /// Creates a new instance.
//        /// </summary>
//        /// <param name="uri">The URI of the WebSocket server.</param>
//        /// <returns></returns>
//        public static WebSocketClient Create(Uri uri)
//        {
//            return new WebSocketClient(uri);
//        }

//        /// <summary>
//        /// Connects to the WebSocket server.
//        /// </summary>
//        /// <returns></returns>
//        public WebSocketClient Connect()
//        {
//            ConnectAsync();
//            return this;
//        }

//        /// <summary>
//        /// Connects to the WebSocket server.
//        /// </summary>
//        /// <returns></returns>
//        public void Disconnect()
//        {
//            Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal", CancellationToken.None).Wait();
//        }

//        public void Dispose()
//        {
//            if (Client != null)
//            {
//                Client.Dispose();
//            }
//        }

//        /// <summary>
//        /// Reconnects to the WebSocket server.
//        /// </summary>
//        /// <returns></returns>
//        public void Reconnect()
//        {
//            if (IsConnected)
//                StartListen();
//            else
//            {
//                ConnectAsync();
//            }
//        }

//        /// <summary>
//        /// Send a message to the WebSocket server.
//        /// </summary>
//        /// <param name="message">The message to send</param>
//        public void SendMessage(string message)
//        {
//            if (Client.State != WebSocketState.Open)
//            {
//                throw new Exception("Connection is not open.");
//            }

//            var messageBuffer = Encoding.UTF8.GetBytes(message);
//            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

//            for (var i = 0; i < messagesCount; i++)
//            {
//                var offset = (SendChunkSize * i);
//                var count = SendChunkSize;
//                var lastMessage = ((i + 1) == messagesCount);

//                if ((count * (i + 1)) > messageBuffer.Length)
//                {
//                    count = messageBuffer.Length - offset;
//                }

//                Client.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, _cancellationToken).Wait();
//            }
//        }

//        private static void RunInTask(Action action)
//        {
//            Task.Run(action);
//        }

//        private void CallOnConnected()
//        {
//            if (OnConnected != null)
//                RunInTask(() => OnConnected(this));
//        }

//        private void CallOnDisconnected()
//        {
//            if (OnDisconnected != null)
//                RunInTask(() => OnDisconnected(this));
//        }

//        private void CallOnError(Exception e)
//        {
//            if (OnError != null)
//                RunInTask(() => OnError(this, e));
//        }

//        private void CallOnMessage(string message)
//        {
//            if (OnMessage != null)
//                RunInTask(() => OnMessage(this, message));
//        }

//        private async void ConnectAsync()
//        {
//            await Client.ConnectAsync(_uri, _cancellationToken);
//            CallOnConnected();
//            StartListen();
//        }

//        private async void StartListen()
//        {
//            var buffer = new byte[ReceiveChunkSize];

//            try
//            {
//                while (Client.State == WebSocketState.Open)
//                {
//                    var stringResult = new StringBuilder();

//                    WebSocketReceiveResult result;
//                    do
//                    {
//                        result = await Client.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

//                        if (result.MessageType == WebSocketMessageType.Close)
//                        {
//                            await
//                                Client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
//                            CallOnDisconnected();
//                        }
//                        else
//                        {
//                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                            stringResult.Append(str);
//                        }
//                    } while (!result.EndOfMessage);

//                    var messages = stringResult
//                        .ToString()
//                        .Split(new string[] { "\r", "\n" }, StringSplitOptions.None)
//                        .Where(c => !string.IsNullOrWhiteSpace(c))
//                        .ToList();

//                    foreach (var msg in messages)
//                    {
//                        CallOnMessage(msg);
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                CallOnError(e);
//                CallOnDisconnected();
//            }
//            finally
//            {
//                if (AutoReconnect)
//                    Reconnect();
//            }
//        }
//    }

//    public class Whisper : MessageData
//    {
//        /// <summary>Whisper object constructor.</summary>
//        public Whisper(string jsonStr)
//        {
//            JObject json = JObject.Parse(jsonStr);
//            Type = json.SelectToken("type").ToString();
//            Data = json.SelectToken("data").ToString();
//            DataObject = new DataObj(json.SelectToken("data_object"));
//        }

//        /// <summary>Data identifier in MessageData</summary>
//        public string Data { get; protected set; }

//        /// <summary>Object that houses the data accompanying the type.</summary>
//        public DataObj DataObject { get; protected set; }

//        /// <summary>Type of MessageData</summary>
//        public string Type { get; protected set; }

//        /// <summary>Class representing the data in the MessageData object.</summary>
//        public class DataObj
//        {
//            /// <summary>DataObj constructor.</summary>
//            public DataObj(JToken json)
//            {
//                Id = int.Parse(json.SelectToken("id").ToString());
//                ThreadId = json.SelectToken("thread_id").ToString();
//                Body = json.SelectToken("body").ToString();
//                SentTs = long.Parse(json.SelectToken("sent_ts").ToString());
//                FromId = long.Parse(json.SelectToken("from_id").ToString());
//                Tags = new TagsObj(json.SelectToken("tags"));
//                Recipient = new RecipientObj(json.SelectToken("recipient"));
//                Nonce = json.SelectToken("nonce").ToString();
//            }

//            /// <summary>Body of data received from Twitch</summary>
//            public string Body { get; protected set; }

//            /// <summary>Id of user that sent whisper.</summary>
//            public long FromId { get; protected set; }

//            /// <summary>DataObject identifier</summary>
//            public int Id { get; protected set; }

//            /// <summary>Uniquely generated string used to identify response from request.</summary>
//            public string Nonce { get; protected set; }

//            /// <summary>Receipient object housing various properties about user who received whisper.</summary>
//            public RecipientObj Recipient { get; protected set; }

//            /// <summary>Timestamp generated by Twitc</summary>
//            public long SentTs { get; protected set; }

//            /// <summary>Tags object housing associated tags.</summary>
//            public TagsObj Tags { get; protected set; }

//            /// <summary>Twitch assigned thread id</summary>
//            public string ThreadId { get; protected set; }

//            /// <summary>Class representing a single badge.</summary>
//            public class Badge
//            {
//                /// <summary></summary>
//                public Badge(JToken json)
//                {
//                    Id = json.SelectToken("id").ToString();
//                    Version = json.SelectToken("version").ToString();
//                }

//                /// <summary>Id of the badge.</summary>
//                public string Id { get; protected set; }

//                /// <summary>Version of the badge.</summary>
//                public string Version { get; protected set; }
//            }

//            /// <summary>Class representing the recipient of the whisper.</summary>
//            public class RecipientObj
//            {
//                /// <summary>RecipientObj constructor.</summary>
//                public RecipientObj(JToken json)
//                {
//                    Id = long.Parse(json.SelectToken("id").ToString());
//                    Username = json.SelectToken("username").ToString();
//                    DisplayName = json.SelectToken("display_name").ToString();
//                    Color = json.SelectToken("color").ToString();
//                    UserType = json.SelectToken("user_type").ToString();
//                    Turbo = bool.Parse(json.SelectToken("turbo").ToString());
//                    Badges = new List<Badge>();
//                    foreach (JToken badge in json.SelectToken("badges"))
//                        Badges.Add(new Badge(badge));
//                }

//                /// <summary>List of badges that the receiver has.</summary>
//                public List<Badge> Badges { get; protected set; }

//                /// <summary>Receiver color.</summary>
//                public string Color { get; protected set; }

//                /// <summary>Receiver display name.</summary>
//                public string DisplayName { get; protected set; }

//                /// <summary>Receiver id</summary>
//                public long Id { get; protected set; }

//                /// <summary>True or false on whther receiver has turbo or not.</summary>
//                public bool Turbo { get; protected set; }

//                /// <summary>Receiver username</summary>
//                public string Username { get; protected set; }

//                /// <summary>User type of receiver.</summary>
//                public string UserType { get; protected set; }
//            }

//            /// <summary>Class representing the tags associated with the whisper.</summary>
//            public class TagsObj
//            {
//                /// <summary>All badges associated with the whisperer</summary>
//                public List<Badge> Badges = new List<Badge>();

//                /// <summary>List of emotes found in whisper</summary>
//                public List<EmoteObj> Emotes = new List<EmoteObj>();

//                /// <summary></summary>
//                public TagsObj(JToken json)
//                {
//                    Login = json.SelectToken("login").ToString();
//                    DisplayName = json.SelectToken("login").ToString();
//                    Color = json.SelectToken("color").ToString();
//                    UserType = json.SelectToken("user_type").ToString();
//                    Turbo = bool.Parse(json.SelectToken("turbo").ToString());
//                    foreach (JToken emote in json.SelectToken("emotes"))
//                        Emotes.Add(new EmoteObj(emote));
//                    foreach (JToken badge in json.SelectToken("badges"))
//                        Badges.Add(new Badge(badge));
//                }

//                /// <summary>Color of whispers</summary>
//                public string Color { get; protected set; }

//                /// <summary>Display name found in chat.</summary>
//                public string DisplayName { get; protected set; }

//                /// <summary>Login value associated.</summary>
//                public string Login { get; protected set; }

//                /// <summary>True or false for whether whisperer is turbo</summary>
//                public bool Turbo { get; protected set; }

//                /// <summary>User type of whisperer</summary>
//                public string UserType { get; protected set; }

//                /// <summary>Class representing a single emote found in a whisper</summary>
//                public class EmoteObj
//                {
//                    /// <summary>EmoteObj construcotr.</summary>
//                    public EmoteObj(JToken json)
//                    {
//                        Id = int.Parse(json.SelectToken("id").ToString());
//                        Start = int.Parse(json.SelectToken("start").ToString());
//                        End = int.Parse(json.SelectToken("end").ToString());
//                    }

//                    /// <summary>Ending character of emote</summary>
//                    public int End { get; protected set; }

//                    /// <summary>Emote ID</summary>
//                    public int Id { get; protected set; }

//                    /// <summary>Starting character of emote</summary>
//                    public int Start { get; protected set; }
//                }
//            }
//        }
//    }
//}