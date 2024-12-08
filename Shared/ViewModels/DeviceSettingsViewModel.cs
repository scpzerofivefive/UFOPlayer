using ReactiveUI;
using Shared.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ViewModels
{
    public class DeviceSettingsViewModel : ReactiveObject
    {
        public event ScriptCommandEventHandler ActionEvent;

        public bool IsFlipped { get; set; }
        public int MinPower { get; set; } = 0;

        public ReactiveCommand<Unit, Unit> TestCommand { get; }

        public DeviceSettingsViewModel()
        {
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
