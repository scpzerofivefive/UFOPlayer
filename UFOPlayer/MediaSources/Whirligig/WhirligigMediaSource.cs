using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UFOPlayer.MediaSources.Whirligig
{
    public class WhirligigMediaSource : AbstractMediaSource, IDisposable
    {

        private WhirligigConnectionSettings _connectionSettings;

        private readonly Thread _clientLoop;

        private bool _running = true;
        private TcpClient _client;
        private TimeSpan _lastReceivedTimestamp = TimeSpan.MaxValue;

        public WhirligigMediaSource(WhirligigConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;

            _clientLoop = new Thread(ClientLoop);
            _clientLoop.Start();
        }

        private void ClientLoop()
        {
            while (_running)
            {
                try
                {
                    _client = new TcpClient();
                    _client.Connect(_connectionSettings.ToEndpoint());

                    SetConnected(true);

                    using (NetworkStream stream = _client.GetStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                InterpretLine(line);
                            }
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
                    _client.Dispose();
                    _client = null;

                    if (_running)
                        SetConnected(false);
                }
            }
        }

        private void SetConnected(bool isConnected)
        {
            IsConnected = isConnected;
        }

        private void InterpretLine(string line)
        {
            if (!line.StartsWith("S") && !line.StartsWith("P"))
                Debug.WriteLine("Whirligig: " + line);
            if (line.StartsWith("S"))
            {
                OnIsPlaying(false);
                //_timeSource.Pause();
            }
            else if (line.StartsWith("C"))
            {
                string file = line.Substring(2).Trim('\t', ' ', '\"');
                Debug.WriteLine($"Whirligig opened '{file}'");
                OnFileOpened(file);
            }
            else if (line.StartsWith("P"))
            {
                string timeStamp = line.Substring(2).Trim();
                double seconds = ParseWhirligigTimestap(timeStamp);
                TimeSpan position = TimeSpan.FromSeconds(seconds);

                //_timeSource.Play();
                OnIsPlaying(true);

                if (position == _lastReceivedTimestamp)
                    return;
                _lastReceivedTimestamp = position;
                //_timeSource.SetPosition(position);
                OnProgressChanged(position);
            }
            else if (line.StartsWith("dometype")) { }
            else if (line.StartsWith("duration"))
            {
                string timeStamp = line.Substring(10).Trim();
                double seconds = ParseWhirligigTimestap(timeStamp);
                //_timeSource.SetDuration(TimeSpan.FromSeconds(seconds));
                OnDurationChanged(TimeSpan.FromSeconds(seconds));
            }
            else
            {
                Debug.WriteLine("Unknown Parameter: " + line);
            }


        }

        private static readonly CultureInfo[] Cultures;


        static WhirligigMediaSource()
        {
            Cultures = new[]
            {
                CultureInfo.InvariantCulture,
                CultureInfo.InstalledUICulture,
                CultureInfo.CurrentCulture,
                CultureInfo.CurrentUICulture,
                CultureInfo.DefaultThreadCurrentCulture,
                CultureInfo.DefaultThreadCurrentUICulture,
            }.Distinct().ToArray();
        }

        private static double ParseWhirligigTimestap(string timeStamp)
        {
            List<double> potentialValues = new List<double>();

            foreach (CultureInfo culture in Cultures)
            {
                if (double.TryParse(timeStamp, NumberStyles.AllowDecimalPoint, culture, out double value))
                {
                    if (value > 0 && !potentialValues.Contains(value))
                        potentialValues.Add(value);
                }
            }

            if (potentialValues.Count == 0)
                return 0;
            if (potentialValues.Count == 1)
                return potentialValues[0];

            return potentialValues.Min();
        }


        public override void Dispose()
        {
            _running = false;
            IsConnected = false;
            _client?.Dispose();

            if (_clientLoop == null)
                return;

            _clientLoop?.Interrupt();
            if (!_clientLoop.Join(500))
            {
                _clientLoop?.Abort();
            }

            _client = null;
        }
    }
}
