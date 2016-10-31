using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public struct EstablishmentAutoSuggestionDto
    {
        /// <summary>
        /// Urn is populated when suggestion refers to only one
        /// </summary>
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FullAddress { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }

        public string Text => $"{Name} ({City}, {PostCode})";

        public EstablishmentAutoSuggestionDto(string name, string address)
        {
            Id = null;
            Name = name;
            FullAddress = address;
            City = null;
            PostCode = null;
        }
        
        public EstablishmentAutoSuggestionDto(int? id, string name, string address) : this(name, address)
        {
            Id = id;
        }

        public EstablishmentAutoSuggestionDto(int? id, string name, string address, string city) : this(id, name, address)
        {
            City = city;
        }

        public EstablishmentAutoSuggestionDto(int? id, string name, string address, string city, string postCode) : this(id, name, address, city)
        {
            PostCode = postCode;
        }


    }
}
