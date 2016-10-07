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

namespace Edubase.Web.UI.Models
{
    [Validator(typeof(CreateEditEstablishmentModelValidator))]
    public class CreateEditEstablishmentModel
    {
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
        public AddressViewModel Address { get; set; }
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public int? HeadTitleId { get; set; }
        public ContactDetailsViewModel Contact { get; set; }
        public ContactDetailsViewModel ContactAlt { get; set; }
        public int? LAESTAB { get; set; }
        public int? TypeId { get; set; }
        public DateTimeViewModel OpenDate { get; set; }
        public DateTimeViewModel CloseDate { get; set; }
        
        public CreateEditEstablishmentModel()
        {
            Address = new AddressViewModel();
            Contact = new ContactDetailsViewModel();
            ContactAlt = new ContactDetailsViewModel();
            OpenDate = new DateTimeViewModel();
            CloseDate = new DateTimeViewModel();
        }


        
    }
}