using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Spatial
{
    public class Distance : IComparable
    {
        protected double _distanceMiles;
        private const double FACTOR = 1.609344;

        public enum eUnit
        {
            Metres,
            Kilometres,
            Miles
        }

        public Distance(double value, eUnit unit)
        {
            if (unit == eUnit.Kilometres) Kilometres = value;
            else if (unit == eUnit.Metres) Metres = value;
            else Miles = value;
        }

        public Distance(double miles)
        {
            _distanceMiles = miles;
        }

        public Distance()
        {

        }

        public double Kilometres
        {
            get { return _distanceMiles * FACTOR; }
            set { _distanceMiles = value / FACTOR; }
        }

        public double Metres
        {
            get { return Kilometres * 1000; }
            set
            {
                if (value > 0) Kilometres = value / 1000;
                else Kilometres = 0;
            }
        }

        public double Miles
        {
            get { return _distanceMiles; }
            set { _distanceMiles = value; }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var other = obj as Distance;
            if (other != null) return _distanceMiles.CompareTo(other._distanceMiles);
            else throw new ArgumentException($"The object passed in is not of the Distance type, it's of the type '{obj?.GetType()?.Name}'");
        }

    }
}
