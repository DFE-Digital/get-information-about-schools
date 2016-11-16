using Edubase.Common.Formatting.Json;
using Edubase.Data.Entity;
using Edubase.Data.Entity.ComplexTypes;
using Lucene.Net.Linq.Fluent;
using Lucene.Net.Linq.Mapping;
using Ver = Lucene.Net.Util.Version;

namespace Edubase.Services.Lucene
{
    internal class EstablishmentIndexConfig
    {
        public static IDocumentMapper<Establishment> CreateMapping()
        {
            var map = new ClassMap<Establishment>(Ver.LUCENE_30);
            map.Key(p => p.Urn);
            map.Property(p => p.Name).Stored().Analyzed();
            map.Property(p => p.FullAddress).Stored();

            map.Property(p => p.Address)
                .ConvertWith(new JsonTypeConverter<Address>()).NotIndexed().Stored();

            map.Property(p => p.Contact)
                .ConvertWith(new JsonTypeConverter<ContactDetail>()).NotIndexed().Stored();

            map.Property(p => p.ContactAlt)
                .ConvertWith(new JsonTypeConverter<ContactDetail>()).NotIndexed().Stored();

            return map.ToDocumentMapper();
        }
        
    }
}
