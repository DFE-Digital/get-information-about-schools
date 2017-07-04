﻿using Edubase.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    [Serializable]
    public class LinkedEstablishmentModel
    {
        public int? Id { get; set; }
        public int? Urn { get; set; }
        public DateTime? LinkDate { get; set; }
        public string EstablishmentName { get; set; }
        
        public int? LinkTypeId { get; set; }
        

        public LinkedEstablishmentModel()
        {

        }
    }
}
