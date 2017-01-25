using Edubase.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.ComplexTypes
{
    [ComplexType, Serializable]
    public class Person
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; TitleDistilled = value.Distill(); }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; FirstNameDistilled = value.Distill(); }
        }

        private string _middleName;

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; MiddleNameDistilled = value.Distill(); }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; LastNameDistilled = value.Distill(); }
        }

        public string TitleDistilled { get; set; }
        public string FirstNameDistilled { get; set; }
        public string MiddleNameDistilled { get; set; }
        public string LastNameDistilled { get; set; }

        [NotMapped]
        public string FullName => ToString();

        public override string ToString() => StringUtil.ConcatNonEmpties(" ", Title, FirstName, MiddleName, LastName);
    }
}
