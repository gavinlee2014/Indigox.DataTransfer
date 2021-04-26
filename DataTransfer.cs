using Indigox.Common.Logging;
using Indigox.DataTransfer.Database;
using Indigox.DataTransfer.QRCode;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer
{
    public partial class DataTransfer : ServiceBase
    {
        AppServer server;

        private IDictionary<string, string> sessionMapping = new Dictionary<string,string>();
        public DataTransfer()
        {
            InitializeComponent();
            CanPauseAndContinue = true;
            
            server = new AppServer();
            server.NewSessionConnected += Server_NewSessionConnected;
            server.NewRequestReceived += Server_NewRequestReceived;
            server.SessionClosed += Server_SessionClosed;
            try {
                Log.Debug("DataTransfer init");
            }
            catch (Exception e)
            {
                this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject.ToString());
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
                sessionMapping.Remove(keyToRemove);
            }
        }

        private void Server_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            Log.Debug(String.Format("receive from session {0} command:{1} message:{2}", 
                session.RemoteEndPoint.ToString(), requestInfo.Key, requestInfo.Body));
            DispatchCommand(session, requestInfo);
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

            if (sessionMapping.ContainsKey(id))
            {
                AppSession targetSession = session.AppServer.GetSessionByID(sessionMapping[id]);
                if (targetSession != null)
                {
                    SendMessage(targetSession, "AUTH", userName + ":" + pwd);
                }
            }
            else
            {
                Log.Debug(String.Format("id {0} not exist in session list", id));
            }
        }
        private void HandleLogin(string id, AppSession session)
        {
            if (!sessionMapping.ContainsKey(id))
            {
                sessionMapping.Add(id, session.SessionID);
            }
            string baseUrl = ConfigurationManager.AppSettings.Get("AUTH_BASE_URL");
            string url = (baseUrl.IndexOf("?") > 0 ? baseUrl + "&" : baseUrl + "?") + "code=" + id;
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeUtil.GenerateMyQCCode(url).Save(ms, ImageFormat.Bmp);
                byte[] content = ms.GetBuffer();

                SendMessage(session, "CODE", content);
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
        

        private void Server_NewSessionConnected(AppSession session)
        {
            Log.Debug("new session " + session.RemoteEndPoint.ToString());
            session.Send("w");
        }

        protected override void OnStart(string[] args)
        {
            string port = ConfigurationManager.AppSettings["port"];
            
            try
            {
                server.Setup(Convert.ToInt32(port));
                server.Start();
                Log.Debug(String.Format("server started at port {0}", port));
            }
            catch (Exception e)
            {
                this.EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                Log.Error(String.Format("server setup fail at port {0} of error:{1}", port, e.ToString()));
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            server.Stop();
            Log.Debug("server paused");
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            server.Start();
            Log.Debug("server resumed");
        }


        protected override void OnStop()
        {
            server.Stop();
            Log.Debug("server stopped");
        }
    }
}
