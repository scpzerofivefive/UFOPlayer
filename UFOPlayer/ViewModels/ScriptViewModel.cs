using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.IO;
using System.Diagnostics;
using UFOPlayer.Script;
using UFOPlayer.Events;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using UFOPlayer.Util;

namespace UFOPlayer.ViewModels
{
    public partial class ScriptViewModel : ObservableObject, INotifyPropertyChanged
    {

        public ScriptViewModel() {
            EventBus.DurationChangedEvent += HandleDurationChanged;
            EventBus.ProgressChangedEvent += HandleProgressChanged;
            EventBus.PlayingChangedEvent += HandlePlayingChanged;
            EventBus.FileChangedEvent += HandleFileChanged;
            EventBus.PlaybackRateChangedEvent += HandlePlaybackRateChanged;
            Task.Run(async () => { scriptLoop(); });
            Debug.WriteLine("New Thread");
        }

        public async void scriptLoop()
        {
            while (true)
            {
                TimeSpan timeSpan = PlaybackClock.getTimeSpan();
                
                int time = (int)timeSpan.TotalMilliseconds;
                Scrubber = time;
                String newPlaybackTime = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                if (!newPlaybackTime.Equals(PlaybackTime))
                {
                    PlaybackTime = newPlaybackTime;
                }
                int i = nextActionIndex;
                while (i < Actions.Count && Actions[i].Timestamp < time)
                {
                    prevAction = Actions[i];
                    i++;
                }
                if (i != nextActionIndex)
                {
                    EventBus.Instance.InvokeAction(prevAction);
                    nextActionIndex = i;
                }
                await Task.Delay(1);
            }
        }

        private ScriptAction prevAction = new ScriptAction(0, 0, 0);
        private int nextActionIndex = 0;

        private FileLoader fileLoader = new FileLoader();

        public PlaybackClock PlaybackClock { get;} = new PlaybackClock();

        [ObservableProperty]
        private int _scrubber = 1;

        [ObservableProperty]
        private String _playbackTime = "00:00:00";

        [ObservableProperty]
        private List<ScriptAction> _actions = new List<ScriptAction>();

        [ObservableProperty]
        private int _duration;

        [ObservableProperty]
        private String _filename = "Select or drop a CSV File";



        // EVENT HANDLERS ==============================================================

        private void HandleDurationChanged(object sender, TimeSpanEventArg arg)
        {
            Duration = (int) arg.time.TotalMilliseconds;
        }

        private void HandleProgressChanged(object sender, TimeSpanEventArg arg)
        {
            PlaybackClock.SetPosition(arg.time);
            if (arg.time.TotalMilliseconds < prevAction.Timestamp)
            {
                nextActionIndex = 0;
                prevAction = new ScriptAction(0, 0, 0);
            }
        }

        public void HandlePlayingChanged(object sender, BooleanEventArg arg)
        {
            if (arg.Value)
            {
                PlaybackClock.Play();
            } else
            {
                PlaybackClock.Pause();
                EventBus.Instance.InvokeAction(new ScriptAction(0,0));
            }
        }

        public void HandlePlaybackRateChanged(object sender, DoubleEventArg arg)
        {
            PlaybackClock.setPlaybackRate(arg.Value);
        }

        public async void HandleFileChanged(object sender, FileEventArg arg)
        {
            String[] filepaths = await fileLoader.getCorrespondingScriptPaths(arg.Filepath);
            if (filepaths.Length == 0)
            {
                Actions = new List<ScriptAction>();
                return;
            }
            Filename = filepaths[0];
            Task.Run(async () => { Actions = await fileLoader.loadFile(filepaths[0]); });
        }


        //=================================================================================



        public void loadFile(IStorageFile file)
        {

            if (!file.Name.EndsWith(".csv"))
            {
                return;
            }
            Filename = file.Name;
            Task.Run(async () => { 
            Actions = await fileLoader.loadFile(file);
            });
        }

        
    }
}
