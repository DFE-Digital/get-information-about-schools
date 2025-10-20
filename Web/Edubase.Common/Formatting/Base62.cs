using System;

namespace Edubase.Common.Formatting
{
    public static class Base62
    {
        static string ToBase62(int value)
        {
            if (value < 10)
                return value.ToString();
            else if (value < 36)
                return ((char)(value - 10 + 'A')).ToString();
            else if (value < 62)
                return ((char)(value - 36 + 'a')).ToString();
            else
                return ToBase62(value / 62) + ToBase62(value % 62);
        }

        public static string Encode(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException($"Val {value} is negative");

            return ToBase62(value / 62) + ToBase62(value % 62);
        }

        public static int Decode(string code)
        {
            int value = 0;
            foreach (char c in code)
            {
                value *= 62;
                if (c <= '9')
                    value += c - '0';
                else if (c <= 'Z')
                    value += c - 'A' + 10;
                else
                    value += c - 'a' + 36;
            }
            return value;
        }

        public static string FromDate(DateTime date) => Encode(int.Parse(string.Concat(date.Year, date.Month, date.Day)));
    }
}
