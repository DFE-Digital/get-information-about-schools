using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Edubase.Web.UI.Models
{
    public class DateTimeViewModel
    {
        public DateTimeViewModel() { }

        public DateTimeViewModel(DateTime? dateTime, DateTime? time = null)
        {
            if (dateTime.HasValue)
            {
                Day = dateTime.Value.Day;
                Month = dateTime.Value.Month;
                Year = dateTime.Value.Year;
            }

            if (time.HasValue)
            {
                Hour = dateTime.Value.Hour;
                Minute = dateTime.Value.Minute;
            }
        }

        [DisplayName("Day"), Range(1, 31)]
        public int? Day { get; set; }

        [DisplayName("Month"), Range(1, 12)]
        public int? Month { get; set; }

        [DisplayName("Year"), Range(1800, 2100)]
        public int? Year { get; set; }

        [DisplayName("Hour"), Range(0, 23)]
        public int? Hour { get; set; }

        [DisplayName("Minute"), Range(0, 59)]
        public int? Minute { get; set; }

        public string Label { get; set; }

        public DateTime? ToDateTime()
        {
            if (Day.HasValue && Month.HasValue && Year.HasValue && Hour.HasValue && Minute.HasValue)
            {
                try
                {
                    return new DateTime(Year.Value, Month.Value, Day.Value, Hour.Value, Minute.Value, 0);
                }
                catch { }
            }
            if (Day.HasValue && Month.HasValue && Year.HasValue)
            {
                try
                {
                    return new DateTime(Year.Value, Month.Value, Day.Value);
                }
                catch { }
            }
            return null;
        }

        public bool IsValid() => !IsEmpty() && ToDateTime() != null;

        public bool IsEmpty() => !Day.HasValue && !Month.HasValue && !Year.HasValue;

        public bool IsNotEmpty() => !IsEmpty();

        public override string ToString()
        {
            var hourString = "";
            if (Hour.HasValue && Minute.HasValue)
            {
                try
                {
                    hourString = $" {Hour?.ToString("00")}:{Minute?.ToString("00")}";
                }
                catch { }
            }
            if (Day.HasValue && Month.HasValue && Year.HasValue)
            {
                try
                {
                    return $"{Day} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month ?? 0)} {Year}{hourString}";
                }
                catch { }
            }
            return null;
        }
    }
}
