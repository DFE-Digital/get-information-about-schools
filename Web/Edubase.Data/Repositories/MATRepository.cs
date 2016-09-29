using Edubase.Data.Entity;
using Edubase.Data.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Data.Repositories
{
    public class MATRepository
    {
        public IList<MAT> Search(string text, int skip = 0, int take = 10) => Search(text, text, skip, take);

        public IList<MAT> Search(string name, string companiesHouseNumber, int skip, int take)
        {
            return new MAT[] {
                new MAT
                { ClosedDate="01/01/1900", GroupUID=232, CompaniesHouseNumber="123454342", GroupID="5723",
                    GroupName ="This is a test", GroupStatus="", GroupStatusCode="00", GroupType="55", GroupTypeCode="55" }
            };
        }

        public MAT Find(short groupUID) => Search(null, null, 0, 0)[0];
    }
}
