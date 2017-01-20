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

        public void Add(string fieldName, object value)
        {
            Add($"{fieldName} eq {value}");
        }

        public void Add(ODataFilterList inner)
        {
            if(inner.Count > 0) Add(string.Concat("(", inner.ToString(), ")"));
        }

        public override string ToString() => string.Join(_op, this);
        
    }
}
