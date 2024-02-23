using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using UFOPlayer.Events;

namespace UFOPlayer.ViewModels
{
    public class DeviceSettingsViewModel : ViewModelBase
    {
        public bool IsFlipped { get; set; }

        public ReactiveCommand<Unit, Unit> TestCommand { get; } 

        public DeviceSettingsViewModel() {
            TestCommand = ReactiveCommand.Create(() => pingDevice());
        }


        public async void pingDevice()
        {
            EventBus.Instance.InvokeAction(new Script.ScriptAction(0, 0, -10));
            await Task.Delay(5000);
            EventBus.Instance.InvokeAction(new Script.ScriptAction(0, 0, 0));
        }
    }
}
