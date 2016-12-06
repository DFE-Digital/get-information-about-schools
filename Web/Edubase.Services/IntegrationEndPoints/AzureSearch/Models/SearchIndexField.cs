using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common;

namespace Edubase.Services.IntegrationEndPoints.AzureSearch.Models
{
    public class SearchIndexField : Field
    {
        private Dictionary<Type, DataType> _mapping = new Dictionary<Type, DataType>
        {
            [typeof(int)] = DataType.Int32,
            [typeof(long)] = DataType.Int64,
            [typeof(bool)] = DataType.Boolean,
            [typeof(DateTime)] = DataType.DateTimeOffset,
            [typeof(double)] = DataType.Double,
            [typeof(DbGeography)] = DataType.GeographyPoint,
            [typeof(string)] = DataType.String,
            [typeof(byte)] = DataType.Int32,
        };

        [Newtonsoft.Json.JsonIgnore]
        public bool IncludeInSuggester { get; set; }

        private Type _clrType;

        [Newtonsoft.Json.JsonIgnore]
        public Type ClrType
        {
            get
            {
                return _clrType;
            }
            set
            {
                var t = value.GetUnderlyingType();
                Type = _mapping.Get(t);
                if (Type == null) throw new Exception($"Unable to map type '{t}'");
                _clrType = value;
            }
        }
    }
}
