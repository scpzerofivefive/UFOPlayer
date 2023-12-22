using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using UFOPlayer.ViewModels;
using System.Diagnostics;

namespace UFOPlayer
{
    public class NewtonsoftJsonSuspensionDriver : ISuspensionDriver
    {
        private readonly string _file;
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public NewtonsoftJsonSuspensionDriver(string file) => _file = file;

        public IObservable<Unit> InvalidateState()
        {
            if (File.Exists(_file))
                File.Delete(_file);
            return Observable.Return(Unit.Default);
        }

        public IObservable<object> LoadState()
        {
            String lines;
            object state;

            try
            {
                lines = File.ReadAllText(_file);
                state = JsonConvert.DeserializeObject<object>(lines, _settings);
                return Observable.Return(state);
            } catch(Exception ex)
            {
                Debug.WriteLine("ERROR: Error while loading settings");
                return null;
            }
            
        }

        public IObservable<Unit> SaveState(object state)
        {
            var lines = JsonConvert.SerializeObject(state, _settings);
            File.WriteAllText(_file, lines);
            return Observable.Return(Unit.Default);
        }
    }
}
