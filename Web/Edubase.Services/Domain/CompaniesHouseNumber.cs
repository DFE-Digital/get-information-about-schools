using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public struct CompaniesHouseNumber
    {
        string _number;

        public CompaniesHouseNumber(string number)
        {
            _number = number;
        }

        public string Number => _number;

        public static CompaniesHouseNumber Parse(string companiesHouseNumber) => new CompaniesHouseNumber(companiesHouseNumber);
    }
}
