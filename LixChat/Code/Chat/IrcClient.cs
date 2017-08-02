using IRC_Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace IRC_Client
{
    public partial class IRC
    {
        public string Channel
        {
            get;
            private set;
        }

        public void HandleJoin(IrcMessage message)
        {
            if (Channel != null)
                OnUserJoinedChannel(new IrcChannelUserEventArgs(Channel, new IrcUser(message.Prefix)));
        }

        public void HandleKick(IrcMessage message)
        {
            var channel = Channel;
            OnUserKicked(new IrcChannelUserEventArgs(channel, new IrcUser(message.Prefix),
            new IrcUser(message.Prefix), message.Parameters[2]));
        }

        public void HandlePart(IrcMessage message)
        {
            OnUserPartedChannel(new IrcChannelUserEventArgs(Channel,
            new IrcUser(message.Prefix)));
        }
    }

    public partial class IRC
    {
        public void HandlePing(IrcMessage message)
        {
            //ServerNameFromPing = message.Parameters[0];
            SendRawMessage("PONG :{0}", message.Parameters[0]);
        }

        public void HandleQuit(IrcMessage message)
        {
            var user = new IrcUser(message.Prefix);
            if (User.Nick != user.Nick)
            {
                OnUserQuit(new IrcChannelUserEventArgs(Channel, user));
            }
        }

        public void RegisterDefaultHandlers()
        {
            SetHandler("PING", HandlePing);

            SetHandler("QUIT", HandleQuit);

            SetHandler("375", HandleMOTDStart);

            SetHandler("376", HandleEndOfMOTD);
            SetHandler("422", HandleMOTDNotFound);

            SetHandler("JOIN", HandleJoin);
            SetHandler("PART", HandlePart);

            SetHandler("KICK", HandleKick);
        }
    }
}

namespace IRC_Client
{
    public partial class IRC
    {
        public string MOTD { get; set; }

        public void HandleEndOfMOTD(IrcMessage message)
        {
            OnConnectionComplete(new EventArgs());
        }

        public void HandleMOTDNotFound(IrcMessage message)
        {
            OnConnectionComplete(new EventArgs());
        }

        public void HandleMOTDStart(IrcMessage message)
        {
            MOTD = string.Empty;
        }
    }
}

namespace IRC_Client
{
    public partial class IRC
    {
        private int sendCnt = 0;

        public bool SendMessage(string message, bool withDelay, bool isMod = false)
        {
            if (withDelay)
            {
                if (sendCnt++ >= (isMod ? 100 : 20))
                {
                    return false;
                }
            }
            SendRawMessage("PRIVMSG {0} :{1}{2}", "#" + Channel, PrivmsgPrefix, message);
            return true;
        }

        private void ProtectionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendCnt = 0;
        }
    }

    public partial class IRC
    {
        private const int ReadBufferLength = 4096;

        //internal RequestManager RequestManager { get; set; }
        private const string ServerNameFromPing = "tmi.twitch.tv";

        private static readonly byte endLineChar = Encoding.UTF8.GetBytes("\n")[0];

        public IRC(string serverAddress, string Channel, string name, string tokken, bool useSSL = false)
        {
            this.Channel = Channel;
            if (serverAddress == null) throw new ArgumentNullException("serverAddress");

            User = new IrcUser(name, name.ToLower(),
                     "oauth:" + tokken);

            ServerAddress = serverAddress;
            Encoding = Encoding.UTF8;
            // Channels = new ChannelCollection();

            Handlers = new Dictionary<string, MessageHandler>();
            RegisterDefaultHandlers();
            //RequestManager = new RequestManager();
            UseSSL = useSSL;
            WriteQueue = new ConcurrentQueue<string>();

            PrivmsgPrefix = "";
        }

        public delegate void MessageHandler(IrcMessage message);

        public event EventHandler<EventArgs> ConnectionComplete;

        public event EventHandler<Exception> NetworkError;

        public event EventHandler<RawMessageEventArgs> RawMessageRecieved;

        public event EventHandler<RawMessageEventArgs> RawMessageSent;

        public event EventHandler<IrcChannelUserEventArgs> UserJoinedChannel;

        public event EventHandler<IrcChannelUserEventArgs> UserKicked;

        public event EventHandler<IrcChannelUserEventArgs> UserPartedChannel;

        public event EventHandler<IrcChannelUserEventArgs> UserQuit;

        public bool ConnectionCompleteIsNull
        {
            get { return ConnectionComplete == null; }
        }

        public Encoding Encoding { get; set; }

        public bool IgnoreInvalidSSL { get; set; }

        public bool NetworkErrorIsNull
        {
            get { return NetworkError == null; }
        }

        public Stream NetworkStream { get; set; }

        public string PrivmsgPrefix { get; set; }

        public bool RawMessageRecievedIsNull
        {
            get { return ConnectionComplete == null; }
        }

        public bool RawMessageSentIsNull
        {
            get { return RawMessageSent == null; }
        }

        public long ReceivedBytes
        {
            get;
            private set;
        }

        public string ServerAddress
        {
            get
            {
                return ServerHostname + ":" + ServerPort;
            }
            internal set
            {
                string[] parts = value.Split(':');
                if (parts.Length > 2 || parts.Length == 0)
                    throw new FormatException("Server address is not in correct format ('hostname:port')");
                ServerHostname = parts[0];
                if (parts.Length > 1)
                    ServerPort = int.Parse(parts[1]);
                else
                    ServerPort = 6667;
            }
        }

        public IrcUser User { get; set; }

        public bool UserJoinedChannelisNull
        {
            get { return UserJoinedChannel == null; }
        }

        public bool UserJoinedChannelIsNull
        {
            get { return UserJoinedChannel == null; }
        }

        public bool UserKickedIsNull
        {
            get { return UserKicked == null; }
        }

        public bool UserPartedChannelIsNull
        {
            get { return UserPartedChannel == null; }
        }

        public bool UserQuitIsNull
        {
            get { return UserQuit == null; }
        }

        public bool UseSSL { get; private set; }
        private Dictionary<string, MessageHandler> Handlers { get; set; }

        private bool IsWriting { get; set; }

        private System.Timers.Timer PingTimer { get; set; }

        private System.Timers.Timer ProtectionTimer { get; set; }

        private byte[] ReadBuffer { get; set; }
        private int ReadBufferIndex { get; set; }

        private string ServerHostname { get; set; }

        private int ServerPort { get; set; }

        private Socket Socket { get; set; }

        private ConcurrentQueue<string> WriteQueue { get; set; }

        public void ConnectAsync()
        {
            if (Socket != null && Socket.Connected)
                throw new InvalidOperationException("Socket is already connected to server.");

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.ReceiveTimeout = 10000;
            Socket.SendTimeout = 10000;

            ReadBuffer = new byte[ReadBufferLength];
            ReadBufferIndex = 0;

            PingTimer = new System.Timers.Timer(30000);
            PingTimer.Elapsed += PingTimer_Elapsed;

            ProtectionTimer = new System.Timers.Timer(30000);
            ProtectionTimer.Elapsed += ProtectionTimer_Elapsed;
            ProtectionTimer.Start();

            var checkQueue = new System.Timers.Timer(1000);
            checkQueue.Elapsed += (sender, e) =>
            {
                string nextMessage;
                if (WriteQueue.Count > 0)
                {
                    while (!WriteQueue.TryDequeue(out nextMessage)) ;
                    SendRawMessage(nextMessage);
                }
            };
            checkQueue.Start();
            Socket.BeginConnect(ServerHostname, ServerPort, ConnectComplete, null);
        }

        public void Dispose()
        {
            if (NetworkStream != null) NetworkStream.Close();
            if (PingTimer != null) PingTimer.Dispose();
            if (ProtectionTimer != null) ProtectionTimer.Dispose();
            if (Socket != null) Socket.Close();
        }

        public void OnConnectionComplete(EventArgs e)
        {
            if (ConnectionComplete != null) ConnectionComplete(this, e);
        }

        public void OnNetworkError(Exception e)
        {
            if (NetworkError != null) NetworkError(this, e);
        }

        public void OnRawMessageRecieved(RawMessageEventArgs e)
        {
            if (RawMessageRecieved != null) RawMessageRecieved(this, e);
        }

        public void OnRawMessageSent(RawMessageEventArgs e)
        {
            if (RawMessageSent != null) RawMessageSent(this, e);
        }

        public void OnUserJoinedChannel(IrcChannelUserEventArgs e)
        {
            if (UserJoinedChannel != null) UserJoinedChannel(this, e);
        }

        public void OnUserKicked(IrcChannelUserEventArgs e)
        {
            if (UserKicked != null) UserKicked(this, e);
        }

        public void OnUserPartedChannel(IrcChannelUserEventArgs e)
        {
            if (UserPartedChannel != null) UserPartedChannel(this, e);
        }

        public void OnUserQuit(IrcChannelUserEventArgs e)
        {
            if (UserQuit != null) UserQuit(this, e);
        }

        public void Quit()
        {
            try
            {
                SendRawMessage("PART {0}", "#" + Channel);
                try
                {
                    Socket.Disconnect(false);
                    Socket.Close();
                }
                catch { }
                try
                {
                    NetworkStream.Close();
                    NetworkStream = null;
                }
                catch { }
                PingTimer.Dispose();
                if (ProtectionTimer != null) ProtectionTimer.Dispose();
            }
            catch
            {
            }
        }

        public void SendRawMessage(string message, params object[] format)
        {
            try
            {
                if (NetworkStream == null)
                {
                    OnNetworkError(new SocketException((int)SocketError.NotConnected));
                    return;
                }

                message = string.Format(message, format);
                var data = Encoding.GetBytes(message + "\r\n");

                if (!IsWriting)
                {
                    IsWriting = true;
                    NetworkStream.BeginWrite(data, 0, data.Length, MessageSent, message);
                }
                else
                {
                    WriteQueue.Enqueue(message);
                }
            }
            catch { }
        }

        public void SetHandler(string message, MessageHandler handler)
        {
            message = message.ToUpper();
            Handlers[message] = handler;
        }

        internal static DateTime DateTimeFromIrcTime(int time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }

        private void ConnectComplete(IAsyncResult result)
        {
            try
            {
                Socket.EndConnect(result);

                NetworkStream = new NetworkStream(Socket);
                if (UseSSL)
                {
                    if (IgnoreInvalidSSL)
                    {
                        NetworkStream = new SslStream(NetworkStream, false, (sender, certificate, chain, policyErrors) => true);
                    }
                    else
                    {
                        NetworkStream = new SslStream(NetworkStream);
                    }
                    ((SslStream)NetworkStream).AuthenticateAsClient(ServerHostname);
                }

                NetworkStream.BeginRead(ReadBuffer, ReadBufferIndex, ReadBuffer.Length, DataRecieved, null);

                if (!string.IsNullOrEmpty(User.Password))
                {
                    SendRawMessage("PASS {0}", User.Password);
                }
                SendRawMessage("NICK {0}", User.Nick);

                SendRawMessage("USER {0} hostname servername :{1}", User.User, User.RealName);

                SendRawMessage("CAP REQ :twitch.tv/membership", "#" + Channel);

                SendRawMessage("JOIN {0}", "#" + Channel);

                //SendRawMessage("TWITCHCLIENT", "#" + Channel);

                SendRawMessage("CAP REQ :twitch.tv/commands", "#" + Channel);
                SendRawMessage("CAP REQ :twitch.tv/tags", "#" + Channel);

                PingTimer.Start();
            }
            catch (Exception x)
            {
                OnNetworkError(x);
                return;
            }
        }

        private void DataRecieved(IAsyncResult result)
        {
            if (NetworkStream == null)
            {
                OnNetworkError(new SocketException((int)SocketError.NotConnected));
                return;
            }

            int length = 0;
            try
            {
                length = NetworkStream.EndRead(result);
                ReceivedBytes += length;

                string msg = Encoding.GetString(ReadBuffer, 0, length);

                length += ReadBufferIndex;

                ReadBufferIndex = 0;
                while (length > 0)
                {
                    int messageLength = Array.IndexOf(ReadBuffer, endLineChar, 0, length);
                    if (messageLength == -1)
                    {
                        ReadBufferIndex = length;
                        break;
                    }
                    messageLength++;
                    var message = Encoding.GetString(ReadBuffer, 0, messageLength - 2);

                    HandleMessage(message);

                    if (length - messageLength > 0)
                    {
                        Array.Copy(ReadBuffer, messageLength, ReadBuffer, 0, length - messageLength);
                    }
                    length -= messageLength;
                }
                NetworkStream.BeginRead(ReadBuffer, ReadBufferIndex, ReadBuffer.Length - ReadBufferIndex, DataRecieved, null);
            }
            catch (Exception x)
            {
                OnNetworkError(x);
            }
        }

        private void HandleMessage(string rawMessage)
        {
            try
            {
                OnRawMessageRecieved(new RawMessageEventArgs(rawMessage, false));
            }
            catch { }
            try
            {
                var message = new IrcMessage(rawMessage);
                if (Handlers.ContainsKey(message.Command.ToUpper()))
                    Handlers[message.Command.ToUpper()](message);
                else
                {
                }
            }
            catch { }
        }

        private void MessageSent(IAsyncResult result)
        {
            if (NetworkStream == null)
            {
                OnNetworkError(new SocketException((int)SocketError.NotConnected));
                IsWriting = false;
                return;
            }

            try
            {
                NetworkStream.EndWrite(result);
            }
            catch (Exception e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException != null)
                    OnNetworkError(socketException);
                else
                    e.Handle();
                return;
            }
            finally
            {
                IsWriting = false;
            }

            OnRawMessageSent(new RawMessageEventArgs((string)result.AsyncState, true));

            string nextMessage;
            if (WriteQueue.Count > 0)
            {
                while (!WriteQueue.TryDequeue(out nextMessage)) ;
                SendRawMessage(nextMessage);
            }
        }

        private void PingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ServerNameFromPing))
                SendRawMessage("PONG :{0}", ServerNameFromPing);
        }
    }
}

namespace IRC_Client
{
    public class IrcMessage
    {
        public IrcMessage(string rawMessage)
        {
            RawMessage = rawMessage;

            if (rawMessage.StartsWith(":"))
            {
                Prefix = rawMessage.Substring(1, rawMessage.IndexOf(' ') - 1);
                rawMessage = rawMessage.Substring(rawMessage.IndexOf(' ') + 1);
            }

            if (rawMessage.Contains(' '))
            {
                Command = rawMessage.Remove(rawMessage.IndexOf(' '));
                rawMessage = rawMessage.Substring(rawMessage.IndexOf(' ') + 1);

                var parameters = new List<string>();
                while (!string.IsNullOrEmpty(rawMessage))
                {
                    if (rawMessage.StartsWith(":"))
                    {
                        parameters.Add(rawMessage.Substring(1));
                        break;
                    }
                    if (!rawMessage.Contains(' '))
                    {
                        parameters.Add(rawMessage);
                        rawMessage = string.Empty;
                        break;
                    }
                    parameters.Add(rawMessage.Remove(rawMessage.IndexOf(' ')));
                    rawMessage = rawMessage.Substring(rawMessage.IndexOf(' ') + 1);
                }
                Parameters = parameters.ToArray();
            }
            else
            {
                Command = rawMessage;
                Parameters = new string[0];
            }
        }

        public string Command { get; private set; }
        public string[] Parameters { get; private set; }
        public string Prefix { get; private set; }
        public string RawMessage { get; private set; }
    }
}

namespace IRC_Client
{
    public class IrcUser : IEquatable<IrcUser>
    {
        public IrcUser(string host)
            : this()
        {
            if (!host.Contains("@") && !host.Contains("!"))
                Nick = host;
            else
            {
                string[] mask = host.Split('@', '!');
                Nick = mask[0];
                User = mask[1];
                if (mask.Length <= 2)
                {
                    Hostname = "";
                }
                else
                {
                    Hostname = mask[2];
                }
            }
        }

        public IrcUser(string nick, string user)
            : this()
        {
            Nick = nick;
            User = user;
            RealName = User;
            Mode = string.Empty;
        }

        public IrcUser(string nick, string user, string password)
            : this(nick, user)
        {
            Password = password;
        }

        public IrcUser(string nick, string user, string password, string realName)
            : this(nick, user, password)
        {
            RealName = realName;
        }

        internal IrcUser()
        {
            //Channels = new ChannelCollection();
            ChannelModes = new Dictionary<string, char?>();
        }

        public string Hostmask
        {
            get
            {
                return Nick + "!" + User + "@" + Hostname;
            }
        }

        public string Hostname { get; internal set; }
        public string Mode { get; internal set; }
        public string Nick { get; internal set; }

        public string Password { get; internal set; }
        public string RealName { get; internal set; }
        public string User { get; internal set; }
        //public ChannelCollection Channels { get; set; }

        internal Dictionary<string, char?> ChannelModes { get; set; }

        public static bool Match(string mask, string value)
        {
            if (value == null)
                value = string.Empty;
            int i = 0;
            int j = 0;
            for (; j < value.Length && i < mask.Length; j++)
            {
                if (mask[i] == '?')
                    i++;
                else if (mask[i] == '*')
                {
                    i++;
                    if (i >= mask.Length)
                        return true;
                    while (++j < value.Length && value[j] != mask[i]) ;
                    if (j-- == value.Length)
                        return false;
                }
                else
                {
                    if (char.ToUpper(mask[i]) != char.ToUpper(value[j]))
                        return false;
                    i++;
                }
            }
            return i == mask.Length && j == value.Length;
        }

        public bool Equals(IrcUser other)
        {
            return other.Hostmask == Hostmask;
        }

        public override bool Equals(object obj)
        {
            if (obj is IrcUser)
                return Equals((IrcUser)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return Hostmask.GetHashCode();
        }

        public bool Match(string mask)
        {
            if (mask.Contains("!") && mask.Contains("@"))
            {
                if (mask.Contains('$'))
                    mask = mask.Remove(mask.IndexOf('$'));
                var parts = mask.Split('!', '@');
                if (Match(parts[0], Nick) && Match(parts[1], User) && Match(parts[2], Hostname))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return Hostmask;
        }
    }
}

namespace IRC_Client
{
    //internal class RequestManager
    //{
    //    public RequestManager()
    //    {
    //        PendingOperations = new Dictionary<string, RequestOperation>();
    //    }

    //    private Dictionary<string, RequestOperation> PendingOperations { get; set; }

    //    public RequestOperation DequeueOperation(string key)
    //    {
    //        var operation = PendingOperations[key];
    //        PendingOperations.Remove(key);
    //        return operation;
    //    }

    //    public RequestOperation PeekOperation(string key)
    //    {
    //        var realKey = PendingOperations.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
    //        return PendingOperations[realKey];
    //    }

    //    public void QueueOperation(string key, RequestOperation operation)
    //    {
    //        if (PendingOperations.ContainsKey(key))
    //            throw new InvalidOperationException("Operation is already pending.");
    //        PendingOperations.Add(key, operation);
    //    }
    //}

    //internal class RequestOperation
    //{
    //    public RequestOperation(object state, Action<RequestOperation> callback)
    //    {
    //        State = state;
    //        Callback = callback;
    //    }

    //    public Action<RequestOperation> Callback { get; set; }
    //    public object State { get; set; }
    //}
}

namespace IRC_Client.Events
{
    public class IrcChannelUserEventArgs : EventArgs
    {
        internal IrcChannelUserEventArgs(string channel, IrcUser user)
        {
            Channel = channel;
            User = user;
        }

        internal IrcChannelUserEventArgs(string channel, IrcUser kicker, IrcUser user, string reason)
        {
            Channel = channel;
            Kicker = kicker;
            User = user;
            Reason = reason;
        }

        public string Channel { get; set; }

        public IrcUser Kicker { get; set; }
        public string Reason { get; set; }
        public IrcUser User { get; set; }
    }

    public class RawMessageEventArgs : EventArgs
    {
        internal RawMessageEventArgs(string message, bool outgoing)
        {
            Message = message;
            Outgoing = outgoing;
        }

        public string Message { get; set; }

        public bool Outgoing { get; set; }
    }
}