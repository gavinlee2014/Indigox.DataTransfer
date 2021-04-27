using Indigox.Common.Logging;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Indigox.DataTransfer
{
    class TaskTimer
    {
        private static TaskTimer instance = new TaskTimer();

        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private TaskTimer()
        {
            timer.Enabled = true;
            timer.Interval = 10 * 60 * 1000;//执行间隔时间,单位为毫秒  
            timer.Elapsed += new ElapsedEventHandler(Timer1_Elapsed);
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            _ = SendSyncRequestAsync();
        }

        private async Task SendSyncRequestAsync()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            try
            {

                string commandPath = ConfigurationManager.AppSettings.Get("AWAKE_URL");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, commandPath);

                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMilliseconds(5 * 60 * 1000);
                Log.Debug("awake send request " + commandPath);

                HttpResponseMessage res = await client.SendAsync(request);
                if (!res.IsSuccessStatusCode)
                {
                    Log.Error("awake response err:" + res.ReasonPhrase + " " + res.StatusCode);
                }
                else
                {
                    Log.Error("awake success");
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == token.Token)
                {
                    Log.Error(ex.ToString());
                }
                else
                {
                    // a web request timeout
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

        }

        public static TaskTimer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TaskTimer();
                }
                return instance;
            }
        }

        public void Start()
        {
            timer.Start();
            _ = SendSyncRequestAsync();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
