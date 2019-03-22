using System;

using PomiDone.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PomiDone.Views
{
    public sealed partial class StatisticsPage : Page
    {
        public StatisticsViewModel ViewModel { get; } = new StatisticsViewModel();

        public StatisticsPage()
        {
            InitializeComponent();
        }
    }
}
