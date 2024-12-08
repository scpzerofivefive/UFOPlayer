using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Shared.MediaSources;
using Shared.Scripts;

namespace UFOPlayer.ViewModels
{
    public partial class ScriptViewModel : ObservableObject, INotifyPropertyChanged
    {

        private AbstractMediaSource _mediaSource;
        public AbstractMediaSource MediaSource { 
            get { return _mediaSource; }
            set
            {
                if (_mediaSource != null)
                {
                    _mediaSource.FileOpenedEvent -= handleFileOpened;
                }
                _mediaSource = value;
                ScriptHandler.MediaSource = value;

                if (_mediaSource != null)
                {
                    _mediaSource.FileOpenedEvent += handleFileOpened;
                }

            }
        }

        private CSVFileLoader _csvFileLoader;

        public ScriptHandler ScriptHandler { get; }

        public ScriptViewModel(ScriptHandler scriptHandler)
        {
            ScriptHandler = scriptHandler;

            _csvFileLoader = new CSVFileLoader();
        }



        //=================================================================================


        public async void handleFileOpened(string file)
        {
            Debug.WriteLine("ASd: " + MainWindowViewModel.Settings.Regex);
            String[] filepaths = await _csvFileLoader.getCorrespondingScriptPaths(file, MainWindowViewModel.Settings.Regex);
            if (filepaths.Length == 0)
            {
                ScriptHandler.Script = new UFOScript();
                return;
            }
            Task.Run(async () => { ScriptHandler.Script = await _csvFileLoader.loadFile(filepaths[0]); });
        }

        public void loadFile(IStorageFile file)
        {

            if (!file.Name.EndsWith(".csv"))
            {
                return;
            }
            Task.Run(async () =>
            {
                ScriptHandler.Script = await _csvFileLoader.loadFile(file.Name, await file.OpenReadAsync());
            });
        }


        [ObservableProperty]
        private VisualizerMode _mode = VisualizerMode.Line;


    }
}
