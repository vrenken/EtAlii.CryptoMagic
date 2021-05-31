namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazorise.Charts;
    using Blazorise.Charts.Streaming;
    using Microsoft.EntityFrameworkCore;

    public partial class CircularView
    {
        private LineChart<LiveDataPoint> _lineChart;
        private dynamic _lineChartOptions; 
        private ChartStreamingOptions _chartStreamingOptions;

        private readonly TimeSpan _chartWidth = TimeSpan.FromDays(1);
        private readonly TimeSpan _sampleFrequency = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _sampleDelay = TimeSpan.FromMinutes(5);

        private readonly string[] _labels = { "Green", "Blue", "Yellow", "Red", "Purple", "Orange" };

        private readonly List<string> _backgroundColors = new()
        {
            ChartColor.FromRgba( 75, 192, 192, 0.5f ), 
            ChartColor.FromRgba( 54, 162, 235, 0.5f ), 
            ChartColor.FromRgba( 255, 206, 86, 0.5f ), 
            ChartColor.FromRgba( 255, 99, 132, 0.5f ), 
            ChartColor.FromRgba( 153, 102, 255, 0.5f ), 
            ChartColor.FromRgba( 255, 159, 64, 0.5f )
        };
        private readonly List<string> _borderColors = new()
        {
            ChartColor.FromRgba( 75, 192, 192, 1f ), 
            ChartColor.FromRgba( 54, 162, 235, 1f ), 
            ChartColor.FromRgba( 255, 206, 86, 1f ), 
            ChartColor.FromRgba( 255, 99, 132, 1f ), 
            ChartColor.FromRgba( 153, 102, 255, 1f ), 
            ChartColor.FromRgba( 255, 159, 64, 1f )
        };

        private ChartStreamingOptions CreateChartStreamingOptions()
        {
            return new()
            {
                FrameRate = 1,
                Refresh = (int)_sampleFrequency.TotalMilliseconds, 
                Duration = (int)_chartWidth.TotalMilliseconds, 
                Delay = (int)_sampleDelay.TotalMilliseconds, 
            };
        }
        
        private object CreateLineChartOptions()
        {
            return new
            {
                Title = new
                {
                    Display = true,
                    Text = $"Circular differences between {Model.FirstSymbol} and {Model.SecondSymbol}"
                },
                Scales = new
                {
                    YAxes = new object[]
                    {
                        new SettableAxis { ScaleLabel = new { LabelString = Model.ReferenceSymbol }, display = false, min = 0m, max = 1m },
                        new SettableAxis { ScaleLabel = new { LabelString = Model.ReferenceSymbol }, display = false, min = 0m, max = 1m },
                        new SettableAxis { ScaleLabel = new { LabelString = Model.ReferenceSymbol }, display = false, min = 0m, max = 1m }
                    },
                    // XAxes = new object[]
                    // {
                    //     new { type = "time", scaleId = "Time"  }
                    // }
                },
                Tooltips = new
                {
                    Mode = "nearest",
                    Intersect = false
                },
                Hover = new
                {
                    Mode = "nearest",
                    Intersect = false
                },
                Animation = false
            };
        }
        
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await HandleRedraw( _lineChart, new Func<LineChartDataset<LiveDataPoint>>[]
                {
                    GetLineChartDatasetForFirstSymbol, 
                    GetLineChartDatasetForSecondSymbol,
                    GetLineChartDatasetForDifference
                });
            }
        }

        private LineChartDataset<LiveDataPoint> GetLineChartDatasetForFirstSymbol()
        {
            var maxNumberOfRecords = (int)(_chartWidth / Model.SampleInterval);
            
            using var data = new DataContext();
            var snapshots = data.Snapshots
                .AsNoTracking()
                .Include(s => s.Trading)
                .Where(s => s.Trading == Model)
                .OrderByDescending(s => s.Moment)
                .Take(maxNumberOfRecords)
                .Select(s => new LiveDataPoint { X = s.Moment, Y = s.SecondSymbolMarketPrice} )
                .Reverse()
                .ToList();

            _lineChartOptions.Scales.YAxes[0].min = snapshots.Min(s => s.Y);
            _lineChartOptions.Scales.YAxes[0].max = snapshots.Max(s => s.Y);
            
            return new SettableChartDataset  
            {
                // yAxisID = "0",
                // xAxisID = "0",
                Data = snapshots,
                PointRadius = 0,
                Label = Model.FirstSymbol,
                BackgroundColor = _backgroundColors[0],
                BorderColor = _borderColors[0],
                Fill = false,
                LineTension = 0,
                //BorderDash = new List<int> { 8, 4 },
            };
        }
        
        private LineChartDataset<LiveDataPoint> GetLineChartDatasetForSecondSymbol()
        {
            var maxNumberOfRecords = (int)(_chartWidth / Model.SampleInterval);
            
            using var data = new DataContext();
            var snapshots = data.Snapshots
                .AsNoTracking()
                .Include(s => s.Trading)
                .Where(s => s.Trading == Model)
                .OrderByDescending(s => s.Moment)
                .Take(maxNumberOfRecords)
                .Select(s => new LiveDataPoint { X = s.Moment, Y = s.FirstSymbolMarketPrice} )
                .Reverse()
                .ToList();

            _lineChartOptions.Scales.YAxes[1].min = snapshots.Min(s => s.Y);
            _lineChartOptions.Scales.YAxes[1].max = snapshots.Max(s => s.Y);
                
            return new SettableChartDataset  
            {
                // yAxisID = "1",
                // xAxisID = "0",
                Data = snapshots,
                PointRadius = 0,
                Label = Model.SecondSymbol,
                BackgroundColor = _backgroundColors[1],
                BorderColor = _borderColors[1],
                Fill = false,
                LineTension = 0,
                //CubicInterpolationMode = "monotone",
            };
        }

        private LineChartDataset<LiveDataPoint> GetLineChartDatasetForDifference()
        {
            var maxNumberOfRecords = (int)(_chartWidth / Model.SampleInterval);
            
            using var data = new DataContext();
            var snapshots = data.CircularTransactions
                .AsNoTracking()
                .Include(s => s.Trading)
                .Where(s => s.Trading == Model)
                .OrderByDescending(s => s.Step)
                .Take(maxNumberOfRecords)
                .Select(s => new LiveDataPoint { X = s.LastCheck.Value, Y = s.Difference} )
                .Reverse()
                .ToList();

            _lineChartOptions.Scales.YAxes[2].min = snapshots.Min(s => s.Y);
            _lineChartOptions.Scales.YAxes[2].max = snapshots.Max(s => s.Y);
                
            return new SettableChartDataset  
            {
                // yAxisID = "2",
                // xAxisID = "0",
                Data = snapshots,
                PointRadius = 0,
                Label = "Difference",
                BackgroundColor = _backgroundColors[2],
                BorderColor = _borderColors[2],
                Fill = false,
                LineTension = 0,
                //CubicInterpolationMode = "monotone",
            };
        }
        
        private async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>(
            BaseChart<TDataSet, TItem, TOptions, TModel> chart, 
            params Func<TDataSet>[] getDataSets)
            where TDataSet : ChartDataset<TItem>
            where TOptions : ChartOptions
            where TModel : ChartModel
        {
            await chart.Clear();

            await chart.AddLabelsDatasetsAndUpdate( _labels, getDataSets.Select( x => x.Invoke()).ToArray());
        }
        
        private async Task OnLineRefreshed(ChartStreamingData<LiveDataPoint> chartData)
        {
            await using var data = new DataContext();
            var snapshot = await data.Snapshots
                .AsNoTracking()
                .Include(s => s.Trading)
                .Where(s => s.Trading == Model)
                .OrderBy(s => s.Moment)
                .LastAsync();
            
            if (chartData.DatasetIndex == 0)
            {
                chartData.Value = new LiveDataPoint
                {
                    X = DateTime.Now,
                    Y = snapshot.FirstSymbolMarketPrice,
                };
                _lineChartOptions.Scales.YAxes[0].min = Math.Min(snapshot.FirstSymbolMarketPrice, _lineChartOptions.Scales.YAxes[0].min);
                _lineChartOptions.Scales.YAxes[0].max = Math.Max(snapshot.FirstSymbolMarketPrice, _lineChartOptions.Scales.YAxes[0].max);
            }
            else if(chartData.DatasetIndex == 1)
            {
                chartData.Value = new LiveDataPoint
                {
                    X = DateTime.Now,
                    Y = snapshot.SecondSymbolMarketPrice,
                };
                _lineChartOptions.Scales.YAxes[1].min = Math.Min(snapshot.SecondSymbolMarketPrice, _lineChartOptions.Scales.YAxes[1].min);
                _lineChartOptions.Scales.YAxes[1].max = Math.Max(snapshot.SecondSymbolMarketPrice, _lineChartOptions.Scales.YAxes[1].max);
            }
            else 
            {
                chartData.Value = new LiveDataPoint
                {
                    X = DateTime.Now,
                    Y = Current.Difference,
                };
                _lineChartOptions.Scales.YAxes[2].min = Math.Min(Current.Difference, _lineChartOptions.Scales.YAxes[2].min);
                _lineChartOptions.Scales.YAxes[2].max = Math.Max(Current.Difference, _lineChartOptions.Scales.YAxes[2].max);
            }

            await InvokeAsync(async () => await _lineChart.SetOptions(_lineChartOptions));
        }
    }
}