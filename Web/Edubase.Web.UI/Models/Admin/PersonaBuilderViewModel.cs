using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Admin
{
    public class PersonaBuilderViewModel
    {
        public class PersonaUser
        {
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public Assertion Assertion { get; set; }
        }

        public class Assertion
        {
            public string NameId { get; set; }
            public AttributeStatement[] AttributeStatements { get; set; }
        }

        public class AttributeStatement
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }

    
}