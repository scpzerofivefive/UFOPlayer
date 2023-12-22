using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tmds.DBus.Protocol;
using UFOPlayer.ViewModels;

namespace UFOPlayer.Views
{
    public partial class ScriptView : UserControl
    {

        public static FilePickerFileType CSV { get; } = new("CSV")
        {
            Patterns = new[] { "*.csv"}
        };

        public ScriptView()
        {
            InitializeComponent();
            AddHandler(DragDrop.DropEvent, Drop);
        }
        public void ClickHandler(object sender, RoutedEventArgs args)
        {
            
        }
        public async void OpenFileButton_Clicked(object? sender, RoutedEventArgs args)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this);
            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open CSV File",
                AllowMultiple = false,
                FileTypeFilter = new[] {CSV}
            });
            if (files.Count >= 1)
            {
                ScriptViewModel viewModel = (ScriptViewModel) DataContext;
                viewModel.loadFile(files[0]);
                /*
                // Open reading stream from the first file.
                await using var stream = await files[0].OpenReadAsync();
                using var streamReader = new StreamReader(stream);
                // Reads all the content of file as a text.
                var fileContent = await streamReader.ReadToEndAsync(); */
            }
        }
        private void Drop(object sender, DragEventArgs e)
        {

            IStorageItem file = e.Data.GetFiles().First();
            Debug.WriteLine(file.Name);
            
            if (file is IStorageFile)
            {
                ScriptViewModel viewModel = (ScriptViewModel)DataContext;
                viewModel.loadFile((IStorageFile) file);
            }
            
        }
    }
}
