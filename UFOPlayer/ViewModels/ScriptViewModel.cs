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
using UFOPlayer.Events;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using UFOPlayer.Scripts;

namespace UFOPlayer.ViewModels
{
    public partial class ScriptViewModel : ObservableObject, INotifyPropertyChanged, IDisposable
    {

        private CancellationTokenSource _cancellationTokenSource;

        public ScriptViewModel()
        {
            EventBus.DurationChangedEvent += HandleDurationChanged;
            EventBus.ProgressChangedEvent += HandleProgressChanged;
            EventBus.PlayingChangedEvent += HandlePlayingChanged;
            EventBus.FileChangedEvent += HandleFileChanged;
            EventBus.PlaybackRateChangedEvent += HandlePlaybackRateChanged;

            // Initialize the cancellation token source
            _cancellationTokenSource = new CancellationTokenSource();

            // Start the script loop in a new task
            Task.Run(async () =>
            {
                await ScriptLoop(_cancellationTokenSource.Token);
            });

            Debug.WriteLine("New Thread");
        }

        public async Task ScriptLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {

                TimeSpan timeSpan = PlaybackClock.getTimeSpan();
                int currentTimeMs = (int)timeSpan.TotalMilliseconds;
                Scrubber = currentTimeMs;

                // Format playback time
                string newPlaybackTime = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";

                // Update playback time only if changed
                if (newPlaybackTime != PlaybackTime)
                {
                    PlaybackTime = newPlaybackTime;
                }

                // Process actions based on current playback time
                while (nextActionIndex < Actions.Count && Actions[nextActionIndex].Timestamp < currentTimeMs)
                {
                    prevAction = Actions[nextActionIndex];
                    EventBus.Instance.InvokeAction(prevAction);
                    nextActionIndex++;
                }

                await Task.Delay(5);
            }
        }

        private ScriptAction prevAction = new ScriptAction(0, 0, 0);
        private int nextActionIndex = 0;

        private FileLoader fileLoader = new FileLoader();

        public MediaSyncController PlaybackClock { get;} = new MediaSyncController();

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

        public void Dispose()
        {
            // Cancel the script loop if needed
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public void loadFile(IStorageFile file)
        {
            EventBus.Instance.InvokeAction(new ScriptAction(0, 0));
            if (!file.Name.EndsWith(".csv"))
            {
                return;
            }
            Filename = file.Name;
            Task.Run(async () => { 
            Actions = await fileLoader.loadFile(file);
            });
        }

        [ObservableProperty]
        private VisualizerMode _mode = VisualizerMode.Line;
    }
}
