using System;
using System.Collections.ObjectModel;

using PomiDone.Core.Models;
using PomiDone.Core.Services;
using PomiDone.Helpers;

namespace PomiDone.ViewModels
{
    public class ChartViewModel : Observable
    {
        public ChartViewModel()
        {
        }

        public ObservableCollection<DataPoint> Source
        {
            get
            {
                // TODO WTS: Replace this with your actual data
                return SampleDataService.GetChartSampleData();
            }
        }
    }
}
