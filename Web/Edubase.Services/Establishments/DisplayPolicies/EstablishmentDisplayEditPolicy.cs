using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class EstablishmentDisplayEditPolicy : EstablishmentFieldList
    {
        public string HeadteacherLabel { get; set; } = "Headteacher/Principal";
        public string HeadEmailAddressLabel { get; set; } = "Headteacher/Principal email";
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
                MainEmailAddressLabel = "Centre email";
                EstablishmentTypeLabel = "Provider type";
            }

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