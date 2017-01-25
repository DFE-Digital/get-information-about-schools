using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data
{
    public class ODataFilterList : List<string>
    {
        public const string AND = " and ";
        public const string OR = " or ";
        public const string GE = "ge";
        public const string GT = "gt";
        public const string LE = "le";
        public const string LT = "lt";


        private string _op;

        public ODataFilterList(string op)
        {
            _op = op;
        }

        public ODataFilterList() : this(AND)
        {

        }

        public ODataFilterList(string op, string initialFilter) : this(op)
        {
            Add(initialFilter);
        }

        public void Add(string fieldName, object value, string odataOperator = "eq")
        {
            string operand = null;
            if (value == null) operand = "null";
            else if (value is string) operand = $"'{value}'";
            else if (value is DateTime) operand = ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ssZ");
            else operand = value.ToString();
            Add($"{fieldName} {odataOperator} {operand}");
        }

        public void Add(ODataFilterList inner)
        {
            if(inner.Count > 0) Add(string.Concat("(", inner.ToString(), ")"));
        }

        public override string ToString() => string.Join(_op, this);
        
    }
}
