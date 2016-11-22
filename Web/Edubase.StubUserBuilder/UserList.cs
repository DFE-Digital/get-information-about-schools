using Edubase.Common;
using Edubase.Services.Security.Permissions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.StubUserBuilder
{
    public class Config
    {
        public bool HideDetails { get; set; } = true;
        public List<User> UserList { get; set; } = new List<User>();
        public string IdpDescription { get; set; } = "Secure Access Simulator";
    }

    public class User
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public Assertion Assertion { get; set; } = new Assertion();
    }

    public class Assertion
    {
        public string NameId { get; set; }
        public List<AttributeStatement> AttributeStatements { get; set; } = new List<AttributeStatement>();
    }

    public class AttributeStatement
    {
        [JsonIgnore]
        public object ValueObject { get; set; }

        private string _value = null;

        public string Type { get; set; }
        public string Value {
            get { return _value ?? (ValueObject != null ? UriHelper.SerializeToUrlToken(ValueObject) : null); }
            set { _value = value; }
        }

        public AttributeStatement()
        {

        }

        public AttributeStatement(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public AttributeStatement(string type, Permission permission)
        {
            Type = type;
            ValueObject = permission;
        }
    }


}