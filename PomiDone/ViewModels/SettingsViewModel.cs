using System;
using System.Windows.Input;

using PomiDone.Helpers;
using PomiDone.Services;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace PomiDone.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : Observable
    {
        private const string WorkTimerSettingsKey = "WorkTimerSettingsKey";
        private const string ShortBreakTimerSettingsKey = "ShortBreakTimerSettingsKey";
        private const string LongBreakTimerSettingsKey = "LongBreakTimerSettingsKey";

        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        public RelayCommand StoreSettings { get; set; }

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        private string _settingsWorkTimer;

        public string SettingsWorkTimer
        {
            get { return _settingsWorkTimer; }

            set { Set(ref _settingsWorkTimer, value); }
        }

        private string _settingsShortBreakTimer;

        public string SettingsShortBreakTimer
        {
            get { return _settingsShortBreakTimer; }

            set { Set(ref _settingsShortBreakTimer, value); }
        }

        private string _settingsLongBreakTimer;

        public string SettingsLongBreakTimer
        {
            get { return _settingsLongBreakTimer; }

            set { Set(ref _settingsLongBreakTimer, value); }
        }

        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        public SettingsViewModel()
        {
            StoreSettings = new RelayCommand(StoreSettingsClickCommandAsync);
            VersionDescription = GetVersionDescription();
            SettingsWorkTimer = StoreTimersService.WorkTimer;
            SettingsShortBreakTimer = StoreTimersService.ShortBreakTimer;
            SettingsLongBreakTimer = StoreTimersService.LongBreakTimer;
        }

        private async void StoreSettingsClickCommandAsync()
        {
            await StoreTimersService.SaveTimerInSettingsAsync(WorkTimerSettingsKey, SettingsWorkTimer);
            await StoreTimersService.SaveTimerInSettingsAsync(ShortBreakTimerSettingsKey, SettingsShortBreakTimer);
            await StoreTimersService.SaveTimerInSettingsAsync(LongBreakTimerSettingsKey, SettingsLongBreakTimer);
            AppRestartFailureReason result = await CoreApplication.RequestRestartAsync("-fastInit -level 1 -foo");
        }

        private void StoreSettingsClickCommand()
        {
            StoreTimersService.SaveTimerInSettings(WorkTimerSettingsKey, SettingsWorkTimer);
            StoreTimersService.SaveTimerInSettings(ShortBreakTimerSettingsKey, SettingsShortBreakTimer);
            StoreTimersService.SaveTimerInSettings(LongBreakTimerSettingsKey, SettingsLongBreakTimer);
        }

        public void Initialize()
        {

        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
