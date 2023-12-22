using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System;
using System.Diagnostics;
using System.Windows.Media;
using UFOPlayer.MediaSources;
using UFOPlayer.ViewModels;
using Windows.Media.Core;
using Avalonia.Media;
using Avalonia.Styling;
using DynamicData.Kernel;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace UFOPlayer.Views
{
    public partial class MediaSourceView : UserControl
    {
        public MediaSourceView()
        {
            InitializeComponent();
            mediaOptions.SelectionChanged += OnComboBoxSelectionChanged;
            this.DetachedFromLogicalTree += detachedHandler;
        }

        private void detachedHandler(object sender, LogicalTreeAttachmentEventArgs e)
        {
            //TODO TEMPORARY
            MediaSourceViewModel viewModel = (MediaSourceViewModel) DataContext;
            if (viewModel.Source  != null) { viewModel.Source.Dispose(); }
            
        }


        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaSourceViewModel viewModel = (MediaSourceViewModel) DataContext;
            viewModel.PlaybackModeChanged((PlaybackMode) e.AddedItems[0]);
        }
    }
}
