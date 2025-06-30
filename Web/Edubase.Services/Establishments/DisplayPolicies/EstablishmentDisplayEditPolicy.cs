using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Newtonsoft.Json;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class EstablishmentDisplayEditPolicy : EstablishmentFieldList
    {
        public string HeadteacherLabel { get; set; } = "Headteacher<span class=\"govuk-visually-hidden\"> </span>/<span class=\"govuk-visually-hidden\"> </span>Principal";
        public string HeadPreferredJobTitleLabel { get; set; } = "Headteacher<span class=\"govuk-visually-hidden\"> </span>/<span class=\"govuk-visually-hidden\"> </span>Principal job title";
        public string HeadEmailAddressLabel { get; set; } = "Headteacher<span class=\"govuk-visually-hidden\"> </span>/<span class=\"govuk-visually-hidden\"> </span>Principal email";
        public string EstablishmentTypeLabel { get; set; } = "School type";
        public string MainEmailAddressLabel { get; set; } = "Email";

        [JsonProperty("iebtDetail")]
        public IEBTDetailDisplayEditPolicy IEBTDetail { get; set; }

        public EstablishmentDisplayEditPolicy Initialise(EstablishmentModel establishment)
        {
            if(establishment.EstablishmentTypeGroupId == (int)eLookupEstablishmentTypeGroup.ChildrensCentres)
            {
                HeadteacherLabel = "Manager";
                HeadEmailAddressLabel = "Manager email";
                EstablishmentTypeLabel = "Provider type";
            }

            if(establishment.TypeId == (int)eLookupEstablishmentType.OnlineProvider)
            {
                HeadteacherLabel = "Headteacher/Manager";
                HeadPreferredJobTitleLabel = "Headteacher/Manager preferred job title";
                HeadEmailAddressLabel = "Headteacher/Manager email address";
                EstablishmentTypeLabel = "Establishment type";
            }

            if (IEBTDetail == null)
            {
                IEBTDetail = new IEBTDetailDisplayEditPolicy();
            }

            IEBTDetail.RegistrationSuspended = establishment.TypeId.OneOfThese(
                    eLookupEstablishmentType.OtherIndependentSchool,
                    eLookupEstablishmentType.OtherIndependentSpecialSchool);

            return this;
        }

        /// <summary>
        /// Returns a list of property names whose values are set to true.
        /// </summary>
        /// <returns></returns>
        public string[] GetTrueFieldNames()
        {
            var retVal = new List<string>();
            retVal.AddRange(GetType().GetProperties().Where(x => x.PropertyType == typeof(bool) && (bool)x.GetValue(this, null)).Select(x => x.Name));
            retVal.AddRange(IEBTDetail.GetTrueFieldNames().Select(x => $"{nameof(IEBTModel)}.{x}"));
            return retVal.ToArray();
        }
    }
}
