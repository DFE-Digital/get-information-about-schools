﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Establishments;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class SelectSharedGovernorViewModel : IEstablishmentPageViewModel
    {
        public string GovernorType { get; set; }

        public eLookupGovernorRole Role { get; set; }

        public IList<GovernorViewModel> Governors { get; set; }

        public string SelectedTab { get; set; }

        public int? Urn { get; set; }

        public string Name { get; set; }

        public TabDisplayPolicy TabDisplayPolicy { get; set; }

        public string Layout { get; set; }

        public int SelectedGovernorId { get; set; }

        public class GovernorViewModel
        {
            public bool Selected { get; set; }
            public bool PreExisting { get; set; }
            public int Id { get; set; }
            public string FullName { get; set; }
            public string AppointingBodyName { get; set; }
            public DateTime? DOB { get; set; }
            public string Nationality { get; set; }
            public string PostCode { get; set; }
            public List<EstablishmentViewModel> SharedWith { get; set; }

            [DisplayName("Date of appointment")]
            public DateTimeViewModel AppointmentStartDate { get; set; }

            [DisplayName("Date term ends")]
            public DateTimeViewModel AppointmentEndDate { get; set; }
        }

        public class EstablishmentViewModel
        {
            public int Urn { get; set; }
            public string EstablishmentName { get; set; }
        }
    }


}