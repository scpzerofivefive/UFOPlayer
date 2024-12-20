﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UFOPlayer.MediaSources.Vlc
{
    public class VlcStatus
    {
        public VlcStatus()
        {

        }
        public VlcStatus(string statusXml)
        {
            try
            {
                XmlDocument status = new XmlDocument();
                status.LoadXml(statusXml);

                string state = status.SelectSingleNode("/root/state")?.InnerText;

                switch (state.ToLower(CultureInfo.InvariantCulture))
                {
                    case "playing":
                        PlaybackState = PlaybackState.Playing;
                        break;
                    case "paused":
                        PlaybackState = PlaybackState.Paused;
                        break;
                    default:
                        PlaybackState = PlaybackState.Stopped;
                        break;
                }

                string length = status.SelectSingleNode("/root/length")?.InnerText;

                double lengthInSeconds = double.Parse(length, CultureInfo.InvariantCulture);

                Duration = TimeSpan.FromSeconds(lengthInSeconds);

                string progress = status.SelectSingleNode("/root/position")?.InnerText;
                string time = status.SelectSingleNode("/root/time")?.InnerText;

                int seconds = int.Parse(time, CultureInfo.InvariantCulture);
                double relativeProgress = double.Parse(progress, CultureInfo.InvariantCulture);

                //Progress = TimeSpan.FromSeconds(lengthInSeconds * relativeProgress);
                Progress = TimeSpan.FromSeconds(seconds);

                string encodedFileName = status.SelectSingleNode("/root/information/category[@name='meta']/info[@name='filename']")?.InnerText;

                if (!string.IsNullOrWhiteSpace(encodedFileName))
                    Filename = WebUtility.HtmlDecode(encodedFileName);
                else
                    Filename = null;

                IsValid = true;
            }
            catch
            {
                IsValid = false;
            }

        }

        public string Filename { get; set; }

        public PlaybackState PlaybackState { get; set; }

        public bool IsValid { get; set; }
        public TimeSpan Duration { get; set; }

        public TimeSpan Progress { get; set; }
    }
}

