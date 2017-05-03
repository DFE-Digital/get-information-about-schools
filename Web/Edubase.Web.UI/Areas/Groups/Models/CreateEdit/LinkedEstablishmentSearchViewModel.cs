﻿using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class LinkedEstablishmentSearchViewModel
    {
        /// <summary>
        /// Urn searched for
        /// </summary>
        public string Urn { get; set; }

        /// <summary>
        /// Name of the establishment
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URN that was found
        /// </summary>
        public int? FoundUrn { get; set; }

        public bool HasResult => FoundUrn.HasValue;

        [Display(Name = "Joined Date")]
        public DateTimeViewModel JoinedDate { get; set; } = new DateTimeViewModel();

        public void Reset()
        {
            Urn = null;
            Name = null;
            FoundUrn = null;
        }
    }
}