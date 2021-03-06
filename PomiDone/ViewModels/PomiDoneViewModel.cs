﻿using System;
using System.Threading;
using PomiDone.Core.Helpers;
using PomiDone.Helpers;
using PomiDone.Services;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace PomiDone.ViewModels
{
    public class PomiDoneViewModel : Observable
    {
        private string _timerTextBlock;
        private string _workTimerTextBlock;
        private string _shortTimerTextBlock;
        private string _longTimerTextBlock;
        private TimeSpan _workTimer;
        private int _workTimerTimeSpanInMinutes = int.Parse(StoreTimersService.WorkTimer);
        private int _shortBreakTimerTimeSpanInMinutes = int.Parse(StoreTimersService.ShortBreakTimer);
        private int _longBreakTimerTimeSpanInMinutes = int.Parse(StoreTimersService.LongBreakTimer);
        private bool _isStarted = false;
        private int _workCounter;
        private int _zeroCrossingCounter;
        private int _shortBreakCounter;
        private int _longBreakCounter;
        private int _timeSpan;
        private int _currentProgress;
        private int _progressMaximum;
        private ExtendedExecutionSession _session = null;
        private Timer _periodicTimer = null;
        private string _currentTask;
        private string _currentTaskStored;
        private string _startPauseResumeIcon;
        public Visibility _stopResetButtonVisibility;

        public PomiDoneViewModel()
        {
            TimerTextBlock = "Initializing...";
            CurrentTask = "Initializing...";
            StartPauseResumeClick = new RelayCommand(StartPauseResumeClickCommand);
            ResetClick = new RelayCommand(ResetClickCommand);
            _workTimer = TimeSpan.FromMinutes(double.Parse(StoreTimersService.WorkTimer));
            _timeSpan = _workTimerTimeSpanInMinutes;
            StartPauseResumeIcon = "Play";
            ProgressMaximum = _timeSpan * 60;
            StopResetButtonVisibility = Visibility.Collapsed;
            BeginExtendedExecution();
        }

        public RelayCommand StartPauseResumeClick { get; set; }
        public RelayCommand ResetClick { get; set; }

        public Visibility StopResetButtonVisibility 
        {
            get { return _stopResetButtonVisibility; }

            set { Set(ref _stopResetButtonVisibility, value); }
        }

        public string StartPauseResumeIcon
        {
            get { return _startPauseResumeIcon; }

            set { Set(ref _startPauseResumeIcon, value); }
        }

        public string CurrentTask
        {
            get { return _currentTask; }

            set { Set(ref _currentTask, value); }
        }

        public string TimerTextBlock
        {
            get { return _timerTextBlock; }

            set { Set(ref _timerTextBlock, value); }
        }

        public string WorkTimerTextBlock
        {
            get { return _workTimerTextBlock; }
            set
            {
                _workTimerTextBlock = value;
                OnPropertyChanged(nameof(WorkTimerTextBlock));
            }
        }

        public string ShortTimerTextBlock
        {
            get { return _shortTimerTextBlock; }
            set
            {
                _shortTimerTextBlock = value;
                OnPropertyChanged(nameof(ShortTimerTextBlock));
            }
        }

        public string LongTimerTextBlock
        {
            get { return _longTimerTextBlock; }
            set
            {
                _longTimerTextBlock = value;
                OnPropertyChanged(nameof(LongTimerTextBlock));
            }
        }

        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }

        public int ProgressMaximum
        {
            get { return _progressMaximum; }
            set
            {
                _progressMaximum = value;
                OnPropertyChanged(nameof(ProgressMaximum));
            }
        }

        private async void OnTimer(object state)
        {
            var timerDisplayFormat = @"m\:ss";
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(
             CoreDispatcherPriority.Normal, () =>
             {
                 // Your UI update code goes here!
                 TimerTextBlock = _workTimer.ToString(timerDisplayFormat);
                 WorkTimerTextBlock = _workCounter.ToString();
                 ShortTimerTextBlock = _shortBreakCounter.ToString();
                 LongTimerTextBlock = _longBreakCounter.ToString();
                 if (!_isStarted)
                 {
                     CurrentTask = "Press Play";
                     return;
                 }

                 _workTimer -= TimeSpan.FromSeconds(1);
                 ProgressMaximum = _timeSpan * 60;
                 CurrentProgress = _timeSpan * 60 - (int)_workTimer.TotalSeconds;
                 TimerTextBlock = _workTimer.ToString(timerDisplayFormat);
                 CurrentTask = _currentTask;
                 if (_workTimer == TimeSpan.Zero)
                 {
                     TimerTextBlock = _workTimer.ToString(timerDisplayFormat);
                     _zeroCrossingCounter++;

                     if (_zeroCrossingCounter % 2 == 0)
                     {
                         if (_workCounter % 4 == 0)
                         {
                             _longBreakCounter++;
                         }
                         else
                         {
                             _shortBreakCounter++;
                         }
                         _timeSpan = _workTimerTimeSpanInMinutes;
                         CurrentTask = "Task";
                         _currentTaskStored = CurrentTask;
                         Singleton<ToastNotificationsService>.Instance.ShowToastNotificationTask();
                     }
                     else
                     {
                         _workCounter++;

                         if (_workCounter % 4 == 0)
                         {
                             _timeSpan = _longBreakTimerTimeSpanInMinutes;
                             CurrentTask = "Long break";
                             _currentTaskStored = CurrentTask;
                             Singleton<ToastNotificationsService>.Instance.ShowToastNotificationLongBreak();
                         }
                         else
                         {
                             _timeSpan = _shortBreakTimerTimeSpanInMinutes;
                             CurrentTask = "Short break";
                             _currentTaskStored = CurrentTask;
                             Singleton<ToastNotificationsService>.Instance.ShowToastNotificationSortBreak();
                         }
                     }
                     _workTimer = TimeSpan.FromMinutes(_timeSpan);
                 }
             });
        }

        private void ClearExtendedExecution()
        {
            if (_session != null)
            {
                _session.Revoked -= SessionRevoked;
                _session.Dispose();
                _session = null;
            }

            if (_periodicTimer != null)
            {
                _periodicTimer.Dispose();
                _periodicTimer = null;
            }
        }

        private async void BeginExtendedExecution()
        {
            // https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/ExtendedExecution
            // The previous Extended Execution must be closed before a new one can be requested.
            // This code is redundant here because the sample doesn't allow a new extended
            // execution to begin until the previous one ends, but we leave it here for illustration.
            ClearExtendedExecution();

            var newSession = new ExtendedExecutionSession();
            newSession.Reason = ExtendedExecutionReason.Unspecified;
            newSession.Description = "Raising periodic timer ticks";
            newSession.Revoked += SessionRevoked;
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    _session = newSession;
                    _periodicTimer = new Timer(OnTimer, DateTime.Now, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                    break;

                default:
                case ExtendedExecutionResult.Denied:
                    newSession.Dispose();
                    break;
            }
        }

        private void EndExtendedExecution()
        {
            ClearExtendedExecution();
        }

        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Reason)
                {
                    case ExtendedExecutionRevokedReason.Resumed:
                        break;

                    case ExtendedExecutionRevokedReason.SystemPolicy:
                        break;
                }

                EndExtendedExecution();
            });
        }

        private void StartPauseResumeClickCommand()
        {
            _isStarted ^= true;
            if (_isStarted)
            {
                StartPauseResumeIcon = "Pause";
                if (_currentTaskStored != null)
                {
                    CurrentTask = _currentTaskStored;
                    StopResetButtonVisibility = Visibility.Collapsed;
                }
                else
                {
                    CurrentTask = "Task";
                    StopResetButtonVisibility = Visibility.Collapsed;
                }
            }
            else
            {
                StartPauseResumeIcon = "Play";
                CurrentTask = "Pausing...";
                StopResetButtonVisibility = Visibility.Visible;
            }
        }

        private void ResetClickCommand()
        {
            _isStarted = false;
            _workTimer = TimeSpan.FromMinutes(_workTimerTimeSpanInMinutes);
            _workCounter = 0;
            _zeroCrossingCounter = 0;
            _shortBreakCounter = 0;
            _longBreakCounter = 0;
            CurrentProgress = 0;
            _timeSpan = _workTimerTimeSpanInMinutes;
            ProgressMaximum = _timeSpan * 60;
            CurrentTask = "Resetting...";
            StartPauseResumeIcon = "Play";
            _currentTaskStored = null;
            StopResetButtonVisibility = Visibility.Collapsed;
        }
    }
}
