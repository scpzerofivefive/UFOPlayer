using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using Windows.Devices.I2c;
using Windows.Web.Http;

namespace UFOPlayer.MediaSources.Vlc
{
    public class VLCMediaSource : AbstractMediaSource
    {
        private VlcConnectionSettings _connectionSettings;

        private Thread _clientLoop;

        private bool _running = true;
        private VlcStatus _previousStatus;



        public VLCMediaSource(VlcConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
            _previousStatus = new VlcStatus { IsValid = false };

            _clientLoop = new Thread(ClientLoop);
            _clientLoop.Start();

        }

        private async void ClientLoop()
        {
            while (_running)
            {
                try
                {
                    while (_running)
                    {
                        string status = await Request("status.xml");
                        if (string.IsNullOrWhiteSpace(status))
                        {

                            SetConnected(false);
                        }
                        else
                        {
                            InterpretStatus(status);
                            SetConnected(true);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                finally
                {
                    if (_running)
                        SetConnected(false);
                }
            }
        }

        private void SetConnected(bool isConnected)
        {
            /*
            if (!CheckAccess())
            { 
                Dispatcher.Invoke(() => { SetConnected(isConnected); });
                return;
            }*/
            IsConnected = isConnected;
        }

        private void InterpretStatus(string statusXml)
        {
            /*
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => InterpretStatus(statusXml));
                return;
            }*/
            try
            {
                VlcStatus newStatus = new VlcStatus(statusXml);

                if (newStatus.IsValid)
                {
                    if (!_previousStatus.IsValid || _previousStatus.Filename != newStatus.Filename)
                    {
                        FindFullFilename(newStatus.Filename);
                    }

                    if (!_previousStatus.IsValid || _previousStatus.PlaybackState != newStatus.PlaybackState)
                    {

                        if (newStatus.PlaybackState == PlaybackState.Playing)
                            OnIsPlaying(true);
                        else
                            OnIsPlaying(false);
                    }

                    if (!_previousStatus.IsValid || _previousStatus.Duration != newStatus.Duration)
                    {
                        OnDurationChanged(newStatus.Duration);
                    }

                    if (!_previousStatus.IsValid || _previousStatus.Progress != newStatus.Progress)
                    {
                        OnProgressChanged(newStatus.Progress);
                    }
                }

                _previousStatus = newStatus;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Couldn't interpret VLC Status: " + exception.Message);
            }
        }

        private async void FindFullFilename(string newStatusFilename)
        {
            string playlist = await Request("playlist.xml");

            if (string.IsNullOrWhiteSpace(playlist))
                return;

            string filename = FindFileByName(playlist, newStatusFilename);

            if (!string.IsNullOrWhiteSpace(filename))
                OnFileOpened(filename);
        }

        public async Task<string> Request(string filename)
        {
            try
            {
                var client = new System.Net.Http.HttpClient();

                string userName = "";
                string password = _connectionSettings.Password;
                string baseUrl = $"http://{_connectionSettings.IpAndPort}";

                //string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
                string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                return await client.GetStringAsync($"{baseUrl}/requests/{filename}");
            }
            catch
            {
                return null;
            }
        }

        private string FindFileByName(string playlist, string newStatusFilename)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(playlist);

            XmlNodeList nodes = document.SelectNodes("//leaf");
            if (nodes == null)
                return null;

            string encodedFilename = null;

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null) continue;
                if (node.Attributes["name"]?.InnerText != newStatusFilename) continue;

                encodedFilename = node.Attributes["uri"]?.InnerText;
                break;
            }

            if (string.IsNullOrWhiteSpace(encodedFilename))
                return null;

            if (encodedFilename.StartsWith("file:///"))
                encodedFilename = encodedFilename.Substring("file:///".Length);

            encodedFilename = HttpUtility.UrlDecode(encodedFilename);
            return encodedFilename;
        }

        public override void Dispose()
        {
            Debug.WriteLine("Destroy Thread");
            _running = false;
            _clientLoop?.Interrupt();
            if (!_clientLoop.Join(1000))
                _clientLoop?.Abort();
        }
    }
}
