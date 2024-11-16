using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using UFOPlayer.Scripts;

namespace UFOPlayer.ViewModels
{
    public class DeviceSettingsViewModel : ViewModelBase
    {
        public event ScriptCommandEventHandler ActionEvent;

        public bool IsFlipped { get; set; }
        public int MinPower { get; set; } = 0;

        public ReactiveCommand<Unit, Unit> TestCommand { get; } 

        public DeviceSettingsViewModel() {
            TestCommand = ReactiveCommand.Create(() => pingDevice());
        }


        public async void pingDevice()
        {

            ActionEvent?.Invoke(new ScriptCommand(0, -10));
            await Task.Delay(5000);
            ActionEvent?.Invoke(new ScriptCommand(0, 0));
        }

    }
}
