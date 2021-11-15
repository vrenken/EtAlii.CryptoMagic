namespace EtAlii.CryptoMagic
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public partial class OneOffProgressDashboard
    {
        private int _currentWeekTotal;
        private int _currentWeekMax;
        private double _averageWeekTotal;

        private int _currentMonthTotal;
        private int _currentMonthMax;
        private double _averageMonthTotal;

        private int _currentYearTotal;
        private int _currentYearMax;
        private double _averageYearTotal;
        private int _total;
        
        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(async () =>
            {
                await using var data = new DataContext();
                var oneOffTradings = await data.OneOffTradings
                    .Where(t => t.IsSuccess)
                    .Where(t => t.End != null)
                    .ToArrayAsync();

                var calendar = new GregorianCalendar();
                var years = oneOffTradings
                    .GroupBy(t => t.End!.Value.Year)
                    .Select(y => new Year
                    {
                        Number = y.Key,
                        DaysInYear = calendar.GetDaysInYear(y.Key),
                        Months = y
                            .GroupBy(t => t.End!.Value.Month)
                            .Select(m => new Month
                            {
                                Number = m.Key,
                                DaysInMonth = calendar.GetDaysInMonth(y.Key, m.Key),
                                Weeks = m
                                    .GroupBy(t => calendar.GetWeekOfYear(t.End!.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                                    .Select(w => new Week
                                    {
                                        Number = w.Key,
                                        Tradings = w.ToArray()
                                    })
                                    .ToArray()
                            })
                            .ToArray()
                    })
                    .ToArray();

                var now = DateTime.Now;

                var currentYear = years.SingleOrDefault(c => c.Number == now.Year);
                
                if (currentYear != null)
                {
                    WriteCurrentYear(currentYear, now, calendar);
                }

                _averageYearTotal = years
                    .Select(y => y
                        .Months.SelectMany(m => m.Weeks.SelectMany(w => w.Tradings))
                        .Count())
                    .Average(); 
                _averageMonthTotal = years
                    .SelectMany(y => y.Months)
                    .Select(m => m.Weeks.Sum(w => w.Tradings.Length))
                    .Average(); 

                _averageWeekTotal = years
                    .SelectMany(y => y.Months)
                    .SelectMany(m => m.Weeks)
                    .Select(w => w.Tradings.Length)
                    .Average(); 

                _total = oneOffTradings.Length;
                
                StateHasChanged();
            });
        }

        private void WriteCurrentYear(Year currentYear, DateTime now, Calendar calendar)
        {
            _currentYearTotal = currentYear.Months
                .SelectMany(m => m.Weeks)
                .SelectMany(w => w.Tradings)
                .Count();
            _currentYearMax = currentYear.DaysInYear;
            
            var currentMonth = currentYear.Months.SingleOrDefault(m => m.Number == now.Month);
            if (currentMonth != null)
            {
                // Write current month.
                WriteCurrentMonth(currentMonth, now, calendar);
                
            }
        }

        private void WriteCurrentMonth(Month currentMonth, DateTime now, Calendar calendar)
        {
            _currentMonthTotal = currentMonth.Weeks
                .SelectMany(w => w.Tradings)
                .Count();
            _currentMonthMax = currentMonth.DaysInMonth;

            var currentNowWeek = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            var currentWeek = currentMonth.Weeks.SingleOrDefault(w => w.Number == currentNowWeek);
            if (currentWeek != null)
            {
                // Write current month.
                WriteCurrentWeek(currentWeek);
            }
        }

        private void WriteCurrentWeek(Week currentWeek)
        {
            _currentWeekTotal = currentWeek.Tradings.Length;
            _currentWeekMax = 7;
        }
    }
}