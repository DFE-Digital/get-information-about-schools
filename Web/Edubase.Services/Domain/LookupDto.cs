using Edubase.Services.Enums;
using Edubase.Common;
using System;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;

namespace Edubase.Services.Domain
{
    public class LookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
        public string Code { get; set; }
        public int? CodeAsInt => Code.ToInteger();

        public LookupDto(LookupBase entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            DisplayOrder = entity.DisplayOrder;
            Code = entity.Code;
        }

        public LookupDto(LocalAuthority entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            DisplayOrder = entity.Order;
        }

        public LookupDto()
        {

        }

        public override string ToString() => Name;
    }
}
