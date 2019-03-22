using System;
using System.Threading.Tasks;

using PomiDone.Helpers;
using PomiDone.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace PomiDone.Services
{
    public class StoreTimersService
    {
        private const string WorkTimerSettingsKey = "WorkTimerSettingsKey";
        private const string ShortBreakTimerSettingsKey = "ShortBreakTimerSettingsKey";
        private const string LongBreakTimerSettingsKey = "LongBreakTimerSettingsKey";

        public static string WorkTimer { get; set; }
        public static string ShortBreakTimer { get; set; }
        public static string LongBreakTimer { get; set; }

        public static async Task InitializeAsync()
        {
            WorkTimer = await LoadTimerFromSettingsAsync(WorkTimerSettingsKey);
            ShortBreakTimer = await LoadTimerFromSettingsAsync(ShortBreakTimerSettingsKey);
            LongBreakTimer = await LoadTimerFromSettingsAsync(LongBreakTimerSettingsKey);
        }

        public static async Task<string> LoadTimerFromSettingsAsync(string key)
        {
            string defaultTimer;

            switch (key)
            {
                case "WorkTimerSettingsKey":
                    defaultTimer = "25";
                    break;
                case "ShortBreakTimerSettingsKey":
                    defaultTimer = "5";
                    break;
                case "LongBreakTimerSettingsKey":
                    defaultTimer = "15";
                    break;
                default:
                    defaultTimer = "5";
                    break;
            }

            string loadedKey = await ApplicationData.Current.LocalSettings.ReadAsync<string>(key);

            if (string.IsNullOrEmpty(loadedKey))
            {
                return defaultTimer;
            }

            return loadedKey;
        }

        public static async Task SaveTimerInSettingsAsync(string key, string timerToStore)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(key, timerToStore);
        }

        public static void SaveTimerInSettings(string key, string timerToStore)
        {
            ApplicationData.Current.LocalSettings.SaveString(key, timerToStore);
        }
    }
}
