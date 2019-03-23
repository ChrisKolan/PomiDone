using Microsoft.Toolkit.Uwp.Notifications;

using Windows.UI.Notifications;

namespace PomiDone.Services
{
    internal partial class ToastNotificationsService
    {
        public void ShowToastNotificationLongBreak()
        {
            // Create the toast content
            var content = new ToastContent()
            {
                // More about the Launch property at https://docs.microsoft.com/dotnet/api/microsoft.toolkit.uwp.notifications.toastcontent
                Launch = "ToastContentActivationParams",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "PomiDone notification"
                            },

                            new AdaptiveText()
                            {
                                Text = @"Take a long break :)"
                            }
                        }
                    }
                },
            };

            // Add the content to the toast
            var toast = new ToastNotification(content.GetXml())
            {
                // TODO WTS: Set a unique identifier for this notification within the notification group. (optional)
                // More details at https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification.tag
                Tag = "ToastTag"
            };

            // And show the toast
            ShowToastNotification(toast);
        }
    }
}
