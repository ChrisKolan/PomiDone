using System;

using PomiDone.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PomiDone.Views
{
    public sealed partial class PomiDonePage : Page
    {
        private static PomiDoneViewModel _viewModel;
        public PomiDoneViewModel ViewModel
        {
            get { return _viewModel ?? (_viewModel = new PomiDoneViewModel()); }
        }

        public PomiDonePage()
        {
            InitializeComponent();
        }
    }
}
