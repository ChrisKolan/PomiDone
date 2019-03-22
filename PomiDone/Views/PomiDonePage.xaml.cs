using System;

using PomiDone.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PomiDone.Views
{
    public sealed partial class PomiDonePage : Page
    {
        public PomiDoneViewModel ViewModel { get; } = new PomiDoneViewModel();

        public PomiDonePage()
        {
            InitializeComponent();
        }
    }
}
