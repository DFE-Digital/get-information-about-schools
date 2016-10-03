using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class DateTimeViewModel
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

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
    }
}