using Edubase.Services.Enums;
using Edubase.Common;
using System;
using Edubase.Data.Entity;

namespace Edubase.Services.Domain
{
    public class LookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
        public string Code { get; set; }
        public int? CodeAsInt => Code.ToInteger();


        public LookupDto()
        {

        }

        public override string ToString() => Name;
    }
}
