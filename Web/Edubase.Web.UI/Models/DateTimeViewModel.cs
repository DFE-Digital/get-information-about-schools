using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

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

        [DisplayName("Day")]
        public int? Day { get; set; }

        [DisplayName("Month")]
        public int? Month { get; set; }

        [DisplayName("Year")]
        public int? Year { get; set; }

        public string Label { get; set; }

        public int[] Days => Enumerable.Range(1, 31).ToArray();

        public int[] Months => Enumerable.Range(1, 12).ToArray();

        public int[] Years
        {
            get
            {
                const int start = 1900;
                return Enumerable.Range(start, DateTime.UtcNow.Year + 5 - start).Reverse().ToArray();
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