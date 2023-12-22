using Avalonia.Interactivity;
using HarfBuzzSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFOPlayer.Script;

namespace UFOPlayer.Events
{
    public delegate void BooleanEventHandler(object sender, BooleanEventArg e);
    public delegate void TimeSpanEventHandler(object sender, TimeSpanEventArg e);
    public delegate void DurationEventHandler(object sender, TimeSpanEventArg e);
    public delegate void FileEventHandler(object sender, FileEventArg e);
    public delegate void DoubleEventHandler(object sender, DoubleEventArg e);
    public delegate void ActionEventHandler(object sender, ActionEventArgs e);
    public delegate void GenericEventHandler(object sender, EventArgs e);
    public class EventBus
    {

        private static EventBus _instance;
        public static EventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventBus();
                }
                return _instance;
            }
        }

        private EventBus() { }

        public static event GenericEventHandler SettingsUpdatedEvent;

        public void InvokeSetttingsUpdate()
        {
            SettingsUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }

        public static event ActionEventHandler ActionEvent;

        public void InvokeAction(ScriptAction action)
        {
            ActionEvent?.Invoke(this, new ActionEventArgs(action));
        }

        public static event BooleanEventHandler PlayingChangedEvent;
        public static event TimeSpanEventHandler ProgressChangedEvent;
        public static event DoubleEventHandler PlaybackRateChangedEvent;
        public static event FileEventHandler FileChangedEvent;
        public static event DurationEventHandler DurationChangedEvent;

        public void InvokePlayingChanged(bool isPlaying)
        {
            Debug.WriteLine("IsPlaing: " + isPlaying);
            PlayingChangedEvent?.Invoke(this, new BooleanEventArg(isPlaying));
        }

        public void InvokeProgressChanged(TimeSpan progress)
        {
            Debug.WriteLine("Progress: " + progress.ToString());
            ProgressChangedEvent?.Invoke(this, new TimeSpanEventArg(progress));
        }

        public void InvokeDurationChanged(TimeSpan duration)
        {
            Debug.WriteLine("Duration: " + duration.ToString());
            DurationChangedEvent?.Invoke(this, new TimeSpanEventArg(duration));
        }

        public void InvokeFileChanged(String e)
        {
            Debug.WriteLine(e.ToString());
            FileChangedEvent?.Invoke(this, new FileEventArg(e));
        }

        public void InvokePlaybackRateChanged(double rate)
        {
            Debug.WriteLine("Playback: " + rate);
            PlaybackRateChangedEvent?.Invoke(this, new DoubleEventArg(rate));
        }



    }


}
