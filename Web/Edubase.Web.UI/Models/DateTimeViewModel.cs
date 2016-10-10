using System;

namespace Edubase.Web.UI.Models
{
    public class DateTimeViewModel
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public DateTimeViewModel() {  }

        public DateTimeViewModel(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                Day = dateTime.Value.Day;
                Month = dateTime.Value.Month;
                Year = dateTime.Value.Year;
            }
        }

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
    }
}