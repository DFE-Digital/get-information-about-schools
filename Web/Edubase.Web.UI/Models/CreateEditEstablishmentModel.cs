using Edubase.Data.Entity;
using Edubase.Web.UI.Models.Validators;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Collections.Generic;
using Edubase.Services;

namespace Edubase.Web.UI.Models
{
    [Validator(typeof(CreateEditEstablishmentModelValidator))]
    public class CreateEditEstablishmentModel
    {
        public enum eAction
        {
            FindEstablishment,
            Save,
            AddLinkedSchool,
            RemoveLinkedSchool
        }

        public enum eLinkType
        {
            Successor,
            Predecessor
        }

        public int? Urn { get; set; }
        public int? LocalAuthorityId { get; set; }
        public string Name { get; set; }
        public int? StatusId { get; set; }
        public int? ReasonEstablishmentOpenedId { get; set; }
        public int? ReasonEstablishmentClosedId { get; set; }
        public int? EducationPhaseId { get; set; }
        public int? StatutoryLowAge { get; set; }
        public int? StatutoryHighAge { get; set; }
        public int? ProvisionBoardingId { get; set; }
        public int? ProvisionNurseryId { get; set; }
        public int? ProvisionOfficialSixthFormId { get; set; }
        public int? GenderId { get; set; }
        public int? ReligiousCharacterId { get; set; }
        public int? ReligiousEthosId { get; set; }
        public string DioceseId { get; set; }
        public int? AdmissionsPolicyId { get; set; }
        public int? Capacity { get; set; }
        public int? ProvisionSpecialClassesId { get; set; }
        public int? UKPRN { get; set; }
        public AddressViewModel Address { get; set; } = new AddressViewModel();
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public int? HeadTitleId { get; set; }
        public ContactDetailsViewModel Contact { get; set; } = new ContactDetailsViewModel();
        public ContactDetailsViewModel ContactAlt { get; set; } = new ContactDetailsViewModel();
        public int? LAESTAB { get; set; }
        public int? TypeId { get; set; }
        public DateTimeViewModel OpenDate { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel CloseDate { get; set; } = new DateTimeViewModel();
        public eAction Action { get; set; }

        public int? LinkedSearchUrn { get; set; }
        public int? LinkedUrnToAdd { get; set; }
        public string LinkedEstabNameToAdd { get; set; }
        public eLinkType? LinkTypeToAdd { get; set; }
        public DateTimeViewModel LinkedDateToAdd { get; set; }
        public int? LinkedItemPositionToRemove { get; set; }
        public List<LinkedEstabViewModel> Links { get; internal set; } = new List<LinkedEstabViewModel>();
        public bool ScrollToLinksSection { get; set; }

        public Dictionary<string, string> SimplifiedLAESTABRules { get; set; }

        public CreateEditEstablishmentModel()
        {
            SimplifiedLAESTABRules = new EstablishmentService().GetSimplifiedRules();
        }

        


    }
}