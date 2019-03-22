using System;

using PomiDone.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PomiDone.Views
{
    public sealed partial class StatisticsPage : Page
    {
        private static SettingsViewModel _viewModel;
        public SettingsViewModel ViewModel
        {
            get { return _viewModel ?? (_viewModel = new SettingsViewModel()); }
        }

        public StatisticsPage()
        {
            InitializeComponent();
        }
    }
}
