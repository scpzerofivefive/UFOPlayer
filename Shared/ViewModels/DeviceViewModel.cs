using Avalonia.Controls.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using InTheHand.Bluetooth;
using ReactiveUI;
using Shared.Scripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;

namespace Shared.ViewModels
{
    public partial class DeviceViewModel : ObservableObject
    {
        public DeviceSettingsViewModel DeviceSettings { get; set; } = new DeviceSettingsViewModel();

        private readonly ScriptHandler _handler;

        private ScriptCommand _command;


        [ObservableProperty]
        private ConnectionStatus _status = ConnectionStatus.Disconnected;
        partial void OnStatusChanged(ConnectionStatus oldValue, ConnectionStatus newValue)
        {
            switch (_status)
            {
                case ConnectionStatus.Disconnected:
                    ButtonTitle = "Connect";
                    Clickable = true;
                    break;
                case ConnectionStatus.Connected:
                    ButtonTitle = "Disconnect";
                    Clickable = true;
                    break;
                case ConnectionStatus.Connecting:
                    ButtonTitle = "Connecting";
                    Clickable = false;
                    break;
            }
        }

        [ObservableProperty]
        private string _buttonTitle = "Connect";

        [ObservableProperty]
        private bool _clickable = true;

        private RemoteGattServer _gattServer;

        public GattCharacteristic gatt { get; set; }

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }

        public DeviceViewModel(ScriptHandler scriptHandler)
        {

            _handler = scriptHandler;
            _handler.ScriptCommandRaised += actionEventHandler;
            DeviceSettings.ActionEvent += actionEventHandler;

            ConnectCommand = ReactiveCommand.Create(() =>
            {
                if (Status == ConnectionStatus.Disconnected)
                {
                    connect();
                }
                else if (Status == ConnectionStatus.Connected)
                {
                    _gattServer.Disconnect();
                    gatt = null;
                    Status = ConnectionStatus.Disconnected;
                }

            }

            );

        }

        public void actionEventHandler(ScriptCommand cmd)
        {
            Debug.WriteLine(string.Format("Actions: ({0},{1})", cmd.Right, cmd.Left));
            if (gatt == null)
                return;

            if (DeviceSettings.IsFlipped)
            {
                cmd = new ScriptCommand(cmd.Right, cmd.Left);
            }

            //Debug.WriteLine(string.Format("Actions: ({0},{1})", cmd.Right, cmd.Left));
            gatt.WriteValueWithoutResponseAsync(cmd.getBuffer());
        }

        public int convertRange(int newMin, int oldVal)
        {
            return newMin + (oldVal - 1) * (100 - newMin) / 99;
        }


        public async void connect()
        {
            Debug.WriteLine("Requesting Bluetooth Device...");
            RequestDeviceOptions options = new RequestDeviceOptions
            {
                Filters = { new BluetoothLEScanFilter { Name = "UFO-TW" } },
                OptionalServices = {
                    new Guid("40ee0100-63ec-4b7f-8ce7-712efd55b90e"),
                    new Guid("40ee0200-63ec-4b7f-8ce7-712efd55b90e"),
                    new Guid("40ee2222-63ec-4b7f-8ce7-712efd55b90e"),
                    new Guid("722a1c08-1a25-4941-6205-bd0fd52415a9")
                },
                AcceptAllDevices = false
            };
            BluetoothDevice device = await Bluetooth.RequestDeviceAsync(options);
            if (device == null)
            {
                Status = ConnectionStatus.Disconnected;
                return;
            }

            /* TODO ScanForDevices doesn't take in filter properly and takes too long.
            var discoveredDevices = await Bluetooth.ScanForDevicesAsync(
                options: options);
            foreach (BluetoothDevice bd in discoveredDevices)
            {
                if (bd.Name == "UFO-TW")
                {
                    device = bd;
                    break;
                }
            }

            Debug.WriteLine(device.Name);
            */


            //var device = devices.First();
            //Debug.WriteLine(device.Name);
            try
            {
                _gattServer = device.Gatt;
                Debug.WriteLine("Connecting to GATT Server...");
                Status = ConnectionStatus.Connecting;
                await _gattServer.ConnectAsync();
                List<GattService> services = await _gattServer.GetPrimaryServicesAsync();
                foreach (GattService service in services)
                {
                    Debug.WriteLine("Service =============================");
                    IReadOnlyList<GattCharacteristic> x = await service.GetCharacteristicsAsync();
                    Debug.WriteLine("UUID: " + service.Uuid);
                    Debug.WriteLine("-------------------------------------");
                    foreach (GattCharacteristic characteristic in x)
                    {
                        Debug.WriteLine(characteristic.Uuid);
                    }
                }
                GattCharacteristic gattChar = await services.Last().GetCharacteristicAsync(
                    BluetoothUuid.FromGuid(new Guid("40ee0202-63ec-4b7f-8ce7-712efd55b90e")));
                Debug.WriteLine(gattChar);
                gatt = gattChar;
                Status = ConnectionStatus.Connected;
            }
            catch (Exception ex)
            {
                Status = ConnectionStatus.Disconnected;
            }

            //await gattChar.WriteValueWithoutResponseAsync(new ScriptAction(0, 10, 10).getBuffer());

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [ObservableProperty]
        private bool _isConnected = false;

        partial void OnStatusChanged(ConnectionStatus status)
        {
            IsConnected = status == ConnectionStatus.Connected;
        }
    }

}
