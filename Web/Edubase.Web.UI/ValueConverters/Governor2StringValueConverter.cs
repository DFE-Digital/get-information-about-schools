using Edubase.Data.Entity;
using Edubase.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Edubase.Common;

namespace Edubase.Web.UI.ValueConverters
{
    using Helpers;

    public class Governor2StringValueConverter
    {
        public string Convert(Governor g, bool isUserLoggedOn)
        {
            var sb = new StringBuilder();
            var role = (eLookupGovernorRole) g.RoleId;
            var attributes = new List<string>();

            sb.Append(g.Person.FullName);
            sb.Append(", ");

            if(!role.OneOfThese(eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.ChiefFinancialOfficer))
                attributes.Add(string.Concat("Appointed by: ", g.AppointingBody?.ToString() ?? "Not recorded"));

            attributes.Add(string.Concat("From: ", g.AppointmentStartDate?.ToString("dd/MM/yyyy") ?? "Not recorded"));
            
            if (!role.OneOfThese(eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.ChiefFinancialOfficer))
                attributes.Add(string.Concat("To: ", g.AppointmentEndDate?.ToString("dd/MM/yyyy") ?? "Not recorded"));

            if (isUserLoggedOn)
            {
                var isFullDisclosureRole = role.OneOfThese(eLookupGovernorRole.ChairOfGovernors, eLookupGovernorRole.ChairOfTrustees, eLookupGovernorRole.Governor, eLookupGovernorRole.Member, eLookupGovernorRole.Trustee);
                
                if (isFullDisclosureRole)
                    if (g.PostCode.Clean() != null)
                        attributes.Add(string.Concat("Home postcode: ", g.PostCode.Clean()));

                if (!role.OneOfThese(eLookupGovernorRole.ChairOfGovernors, eLookupGovernorRole.ChairOfTrustees, 
                    eLookupGovernorRole.ChiefFinancialOfficer, eLookupGovernorRole.AccountingOfficer))
                {
                    if (g.EmailAddress.Clean() != null)
                        attributes.Add(string.Concat("Direct email address: ", g.EmailAddress.Clean()));
                }

                if (isFullDisclosureRole)
                {
                    if (g.DOB.HasValue) attributes.Add(string.Concat("Date of birth: ", g.DOB.Value.ToString("dd/MM/yyyy")));
                    if (g.PreviousPerson.Title != null) attributes.Add(string.Concat("Previous title: ", g.PreviousPerson.Title));
                    if (g.PreviousPerson.FirstName != null) attributes.Add(string.Concat("Previous forename 1: ", g.PreviousPerson.FirstName));
                    if (g.PreviousPerson.MiddleName != null) attributes.Add(string.Concat("Previous forename 2: ", g.PreviousPerson.MiddleName));
                    if (g.PreviousPerson.LastName != null) attributes.Add(string.Concat("Previous surname: ", g.PreviousPerson.LastName));
                    // TODO: add country of birth here
                    if (g.Nationality != null) attributes.Add(string.Concat("Nationality: ", g.Nationality));
                    // TODO: add tel. number here
                }
            }

            sb.Append(string.Join(", ", attributes));

            return sb.ToString();
        }
    }
}