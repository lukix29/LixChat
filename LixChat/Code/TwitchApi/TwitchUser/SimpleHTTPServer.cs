// MIT License - Copyright (c) 2016 Can Güney Aksakalli

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

internal class Token_HTTP_Server
{
    private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region extension to MIME type list

        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion extension to MIME type list
    };

    private readonly string[] _indexFiles = {
        "index.html",
        //"index.htm",
        //"default.html",
        //"default.htm"
    };

    private HttpListener _listener;
    private int _port;
    private string _rootDirectory;
    private Task _serverThread;
    private bool _stopServer = false;

    /// <summary>
    /// Construct server with given port.
    /// </summary>
    /// <param name="path">Directory path to serve.</param>
    /// <param name="port">Port of the server.</param>
    public Token_HTTP_Server(string path, int port)
    {
        this.Initialize(path, port);
    }

    public delegate void OnTokenReceived(string Token);

    public event OnTokenReceived ReceivedToken;

    public bool IsStarted
    {
        get;
        private set;
    }

    public int Port
    {
        get { return _port; }
        private set { }
    }

    ///// <summary>
    ///// Construct server with suitable port.
    ///// </summary>
    ///// <param name="path">Directory path to serve.</param>
    //public Token_HTTP_Server(string path)
    //{
    //    //get an empty port
    //    TcpListener l = new TcpListener(IPAddress.Loopback, 0);
    //    l.Start();
    //    int port = ((IPEndPoint)l.LocalEndpoint).Port;
    //    l.Stop();
    //    this.Initialize(path, port);
    //}

    public string Token
    {
        get;
        private set;
    }

    public void Start()
    {
        if (!IsStarted)
        {
            _stopServer = false;
            _serverThread = Task.Run(() => this.Listen());
            IsStarted = true;
        }
    }

    /// <summary>
    /// Stop server and dispose all functions.
    /// </summary>
    public void Stop()
    {
        _stopServer = true;
        IsStarted = false;
    }

    private void CheckPort()
    {
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
        Random rd = new Random();
        for (int i = 0; i < tcpConnInfoArray.Length; i++)
        {
            var tcpi = tcpConnInfoArray[i];
            if (tcpi.LocalEndPoint.Port == _port)
            {
                _port += rd.Next(-100, 101);
                i = 0;
                break;
            }
        }
    }

    private void Initialize(string path, int port)
    {
        this._rootDirectory = path;
        this._port = port;
    }

    private void Listen()
    {
        try
        {
            if (_listener != null) _listener.Close();
            _listener = new HttpListener();

            CheckPort();

            _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
            _listener.Start();
            while (true)
            {
                if (_stopServer)
                {
                    _listener.Close();
                    return;
                }
                HttpListenerContext context = _listener.GetContext();
                Process(context);
            }
        }
        catch (Exception ex)
        {
            if (IsStarted)
            {
                if (LX29_Tools.HasInternetConnection)
                {
                    Listen();
                }
                else
                {
                    if (!(ex is ThreadAbortException))
                    {
                        switch (ex.Handle())
                        {
                            case System.Windows.Forms.MessageBoxResult.Retry:
                                Listen();
                                break;
                        }
                    }
                }
            }
        }
    }

    private void Process(HttpListenerContext context)
    {
        string filename = context.Request.Url.AbsolutePath;
        //Console.WriteLine(filename);
        if (!filename.IsEmpty())
        {
            if (filename.Contains("token"))
            {
                Token = filename.Replace("token", "").Replace("/", "");

                if (!Token.IsEmpty())
                {
                    WriteFile(_rootDirectory + "received.html", context);
                    if (ReceivedToken != null)
                        System.Threading.Tasks.Task.Run(() => ReceivedToken(Token));
                }
            }
        }

        filename = filename.Substring(1);

        if (filename.IsEmpty())
        {
            foreach (string indexFile in _indexFiles)
            {
                if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                {
                    filename = indexFile;
                    break;
                }
            }
        }

        filename = Path.Combine(_rootDirectory, filename);

        WriteFile(filename, context);

        context.Response.OutputStream.Close();

        if (filename.EndsWith("success.html"))
        {
            Stop();
        }
    }

    private void WriteFile(string filename, HttpListenerContext context)
    {
        if (File.Exists(filename))
        {
            try
            {
                Stream input = new FileStream(filename, FileMode.Open);

                //Adding permanent http response headers
                string mime;
                context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                context.Response.ContentLength64 = input.Length;
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

                byte[] buffer = new byte[1024 * 16];
                int nbytes;
                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, nbytes);
                input.Close();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}