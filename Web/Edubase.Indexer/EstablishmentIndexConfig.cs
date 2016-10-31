using Edubase.Data.Entity;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Linq.Fluent;
using Lucene.Net.Util;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System;
using Ver = Lucene.Net.Util.Version;
using Lucene.Net.Linq.Mapping;

namespace Edubase.Indexer
{
    public class EstablishmentIndexConfig
    {
        public static IDocumentMapper<Establishment> CreateMapping()
        {
            var map = new ClassMap<Establishment>(Ver.LUCENE_30);
            map.Key(p => p.Urn);
            //map.Property(x => x.LocalAuthority).ConvertWith(new LATypeConverter());
            map.Property(p => p.Name).Stored()
                .AnalyzeWith(new StandardAnalyzer(Ver.LUCENE_30))
                .WithTermVector.PositionsAndOffsets();
            return map.ToDocumentMapper();
        }

        public class LATypeConverter: TypeConverter
        {
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return true;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return true;
            }
        }
    }
}
