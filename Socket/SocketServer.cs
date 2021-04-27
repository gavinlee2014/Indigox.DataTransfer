using Indigox.Common.Logging;
using Indigox.DataTransfer.Database;
using Indigox.DataTransfer.QRCode;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Indigox.DataTransfer.Socket
{
    class SocketServer
    {
        private static SocketServer instance = new SocketServer();

        AppServer server;

        private ConcurrentDictionary<string, string> sessionMapping = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, CachedCommand> commandCache = new ConcurrentDictionary<string, CachedCommand>();

        private SocketServer()
        {
            server = new AppServer();
            server.NewSessionConnected += Server_NewSessionConnected;
            server.NewRequestReceived += Server_NewRequestReceived;
            server.SessionClosed += Server_SessionClosed;
            Log.Debug("DataTransfer init");
        }

        public void Start()
        {

            string port = ConfigurationManager.AppSettings["PORT"];
            server.Setup(Convert.ToInt32(port));
            server.Start();
            Log.Debug(String.Format("socket started at port {0}", port));
        }
        public void Stop()
        {
            server.Stop();
            Log.Debug("socket stopped");
        }

        private void Server_SessionClosed(AppSession session, CloseReason value)
        {
            Log.Debug(String.Format("session close {0} of reason {1}", session.RemoteEndPoint.ToString(), value));
            string keyToRemove = "";
            foreach (string key in sessionMapping.Keys)
            {
                if (sessionMapping[key].Equals(session.SessionID))
                {
                    keyToRemove = key;
                    break;
                }
            }
            if (!String.IsNullOrEmpty(keyToRemove))
            {
                sessionMapping.TryRemove(keyToRemove, out string val);
            }
        }
        private void Server_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            Log.Debug(String.Format("receive from session {0} command:{1} message:{2}",
                session.RemoteEndPoint.ToString(), requestInfo.Key, requestInfo.Body));
            DispatchCommand(session, requestInfo);
        }
        private void Server_NewSessionConnected(AppSession session)
        {
            Log.Debug("new session " + session.RemoteEndPoint.ToString());
        }
        private void SendMessage(AppSession session, string command, byte[] content)
        {
            Log.Debug(String.Format("send byte message to session {0} ",
                session.RemoteEndPoint.ToString(), command));
            DoSendMessage(session, command, BuildBase64Response(content));
        }
        private void SendMessage(AppSession session, string command, string message)
        {
            Log.Debug(String.Format("send message to session {0} command:{1} message:{2}",
                session.RemoteEndPoint.ToString(), command, message));
            DoSendMessage(session, command, BuildBase64Response(message));
        }
        private void DoSendMessage(AppSession session, string command, string encodedMessage)
        {
            string response = String.Format("{0} {1}\r\n", command, encodedMessage);
            //Log.Debug(String.Format("encoded response to session {0} body:{1}",
            //    session.RemoteEndPoint.ToString(), encodedMessage));
            session.Send(response);
        }
        private void DispatchCommand(AppSession session, StringRequestInfo requestInfo)
        {
            switch (requestInfo.Key)
            {
                case "LOGIN":
                    HandleLogin(requestInfo.Parameters[0], session);
                    break;
                case "TRANS":
                    HandleTransfer(requestInfo.Parameters, session);
                    break;
                case "PWD":
                    HandlePwd(requestInfo.Parameters, session);
                    break;
                case "LIST":
                    HandleList(requestInfo.Parameters, session);
                    break;
                default:
                    break;
            }
        }
        private void HandleList(string[] parameters, AppSession session)
        {
            string ids = "";
            foreach (string key in sessionMapping.Keys)
            {
                ids += key + " ";
            }
            SendMessage(session, "LIST_R", ids);
        }
        private void HandleTransfer(string[] parameters, AppSession session)
        {
            if (parameters.Length < 2)
            {
                Log.Error("receive wrong TRANS command ");
                return;
            }
            string id = parameters[0];
            string userName = parameters[1];
            string accountName = userName;
            if (userName.IndexOf("\\") > 0)
            {
                accountName = userName.Substring(userName.IndexOf("\\") + 1);
            }
            string pwd = DBUtil.QueryUserPassword(accountName);

            if (sessionMapping.ContainsKey(id) && (server.GetSessionByID(sessionMapping[id]) != null))
            {
                AppSession targetSession = session.AppServer.GetSessionByID(sessionMapping[id]);
                SendMessage(targetSession, "AUTH", userName + ":" + pwd);
            }
            else
            {
                SaveCommandCache(id, "AUTH", userName + ":" + pwd);
                Log.Debug(String.Format("id {0} not exist in session list,saved to cache", id));
            }
        }

        private void SaveCommandCache(string id, string commandName, string message)
        {
            CachedCommand command = new CachedCommand();
            command.ID = id;
            command.CommandName = commandName;
            command.Content = message;
            command.Timestamp = DateTime.Now;
            commandCache.AddOrUpdate(id, command, (key, old) => command);
        }
        private void ResendCommandFromCache(string id)
        {
            if (!commandCache.ContainsKey(id))
            {
                return;
            }
            if ((!sessionMapping.ContainsKey(id)) || (server.GetSessionByID(sessionMapping[id]) == null))
            {
                return;
            }
            AppSession targetSession = server.GetSessionByID(sessionMapping[id]);
            CachedCommand command = commandCache[id];
            TimeSpan span = new TimeSpan(DateTime.Now.Ticks - command.Timestamp.Ticks);
            if (span.TotalSeconds <= 10)
            {
                SendMessage(targetSession, command.CommandName, command.Content);
                Log.Debug(String.Format("resend command to id {0}, command content:{1} {2}", id, command.CommandName, command.Content));
            }
            commandCache.TryRemove(id, out CachedCommand val);
            CleanCommandCache();
        }
        private void CleanCommandCache()
        {
            List<string> keysToRemove = commandCache.Keys.Where(key => new TimeSpan(DateTime.Now.Ticks - commandCache[key].Timestamp.Ticks).TotalSeconds > 10).ToList();
            keysToRemove.ForEach(key => commandCache.TryRemove(key, out CachedCommand val));
        }
        private void HandleLogin(string id, AppSession session)
        {
            sessionMapping.AddOrUpdate(id, session.SessionID, (key, old) => session.SessionID);
            string baseUrl = ConfigurationManager.AppSettings.Get("AUTH_BASE_URL");
            string url = (baseUrl.IndexOf("?") > 0 ? baseUrl + "&" : baseUrl + "?") + "code=" + id;
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeUtil.GenerateMyQCCode(url).Save(ms, ImageFormat.Bmp);
                byte[] content = ms.GetBuffer();

                SendMessage(session, "CODE", content);
            }
            if (commandCache.ContainsKey(id))
            {
                ResendCommandFromCache(id);
            }
        }
        private void HandlePwd(string[] parameters, AppSession session)
        {
            if (parameters.Length < 2)
            {
                return;
            }
            string userName = parameters[0];
            string pwd = parameters[1];
            DBUtil.UpdateUserPassword(userName, pwd);
        }
        private string BuildBase64Response(string message)
        {
            string encodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            return encodedMessage;
        }
        private string BuildBase64Response(byte[] content)
        {
            string encodedMessage = Convert.ToBase64String(content);
            return encodedMessage;
        }
        private ArraySegment<byte> BuildBinResponse(string message)
        {
            byte[] command, bodyLength, body;

            byte[] intBytes = BitConverter.GetBytes((int)CommandType.AUTH);
            Array.Reverse(intBytes);
            command = intBytes;

            body = Encoding.UTF8.GetBytes(message);

            byte[] intLengthBytes = BitConverter.GetBytes(body.Length);
            Array.Reverse(intLengthBytes);
            bodyLength = intLengthBytes;

            byte[] result = command.Concat(bodyLength).Concat(body).ToArray();
            return new ArraySegment<byte>(result);
        }
        public static SocketServer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SocketServer();
                }
                return instance;
            }
        }
    }

    class CachedCommand
    {
        public string ID;
        public string CommandName;
        public string Content;
        public DateTime Timestamp;
    }
}
