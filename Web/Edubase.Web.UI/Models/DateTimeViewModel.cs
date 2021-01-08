using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Globalization

namespace Edubase.Web.UI.Models
{
    public class DateTimeViewModel
    {
        public DateTimeViewModel() { }

        public DateTimeViewModel(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                Day = dateTime.Value.Day;
                Month = dateTime.Value.Month;
                Year = dateTime.Value.Year;
            }
        }

        [DisplayName("Day"), Range(1, 31)]
        public int? Day { get; set; }

        [DisplayName("Month"), Range(1, 12)]
        public int? Month { get; set; }

        [DisplayName("Year"), Range(1800, 2100)]
        public int? Year { get; set; }

        public string Label { get; set; }

        public DateTime? ToDateTime()
        {
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
            var thisMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month ?? 0);
            return $"{Day} {thisMonth} {Year}";
        } 
    }
}
