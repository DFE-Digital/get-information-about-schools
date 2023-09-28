using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Exceptions;

namespace Edubase.Web.UI.Helpers
{
    /// <summary>
    /// This is a utility class that helps with some operations regarding Academy establishment type.
    /// </summary>
    public static class AcademyUtility
    {
        /// <summary>
        /// A method to use to get the words for the title page.
        /// Note that this method expects, if the 'establishmentTypeId' is set to be 46 .
        /// </summary>
        /// <param name="establishmentTypeId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetAcademyOpeningPageTitle(string establishmentTypeId)
        {
            if (string.IsNullOrWhiteSpace(establishmentTypeId)) return "Manage academy openings";

            if (!string.IsNullOrWhiteSpace(establishmentTypeId) &&
                IsSecureAcademy16To19EstablishmentTypeId(establishmentTypeId))
                return "Manage secure academy 16-19 openings";

            throw new ArgumentException("unexpected parameters or values passed");
        }

        /// <summary>
        /// A method to use to filter the passed in Establishments if the Establishment Type Id is set to
        /// the Secure Academy 16 To 19 Establishment Type Id i.e. 46.
        /// Note, it returns the passed in EstablishmentTypes without filtering it if the Establishment Type Id is not set
        /// or if it is not set to the Secure Academy 16 To 19 Establishment Type Id i.e. 46.
        /// </summary>
        /// <param name="establishmentTypes"></param>
        /// <param name="establishmentTypeId"></param>
        /// <returns></returns>
        public static IEnumerable<EstablishmentLookupDto> FilterEstablishmentsIfSecureAcademy16To19(
            IEnumerable<EstablishmentLookupDto> establishmentTypes, string establishmentTypeId)
        {
            if (!string.IsNullOrWhiteSpace(establishmentTypeId) &&
                int.TryParse(establishmentTypeId, out var estabTypeId) &&
                IsSecureAcademy16To19EstablishmentTypeId(establishmentTypeId))
                establishmentTypes = establishmentTypes.Where(e => e.Id == estabTypeId);

            return establishmentTypes;
        }

        /// <summary>
        /// A method to use to get the correct Establishment filters to use in a search.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="establishmentTypeId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static EstablishmentSearchFilters GetEstablishmentSearchFilters(DateTime from, DateTime to,
            string establishmentTypeId)
        {
            //secure 16-19 academy establishment type Id is 46
            var estabTypeId = -1;
            if (!string.IsNullOrWhiteSpace(establishmentTypeId) && int.TryParse(establishmentTypeId, out estabTypeId) &&
                estabTypeId != 46)
                throw new ArgumentException("Invalid parameter(s) supplied");

            if (!string.IsNullOrWhiteSpace(establishmentTypeId) && estabTypeId == 46)
                return new EstablishmentSearchFilters
                {
                    OpenDateMin = from,
                    OpenDateMax = to,
                    StatusIds = new[] { (int) eLookupEstablishmentStatus.ProposedToOpen },
                    TypeIds = new[] { estabTypeId }
                };

            return new EstablishmentSearchFilters
            {
                OpenDateMin = from,
                OpenDateMax = to,
                StatusIds = new[] { (int) eLookupEstablishmentStatus.ProposedToOpen },
                //Academy alternative provision =36, Academy alternative provision =37,
                //Academy special converter =38, Academy 16-19 converter =39, Academy 16 to 19 sponsor led
                TypeIds = new[] { 22, 28, 27, 36, 37, 38, 39, 40 }
            };
        }

        /// <summary>
        /// A method to use to get the value of the role of a user based on the authorization group passed.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authorizedRoles"></param>
        /// <returns></returns>
        public static string GetAuthorizedRole(IPrincipal user,
            string authorizedRoles = AuthorizedRoles.CanManageSecureAcademy16To19Openings)
        {
            var identity = (ClaimsIdentity) user.Identity;
            var roles = identity.Claims.ToList()
                .Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            return roles.FirstOrDefault(role => authorizedRoles.Contains(role));
        }

        /// <summary>
        /// A method to use to check if the user has the authorization to access a resource based
        /// on the authorization role the user is part of.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="establishmentTypeId"></param>
        /// <returns></returns>
        public static bool DoesHaveAccessAuthorization(IPrincipal user, string establishmentTypeId)
        {
            if (string.IsNullOrWhiteSpace(establishmentTypeId) && IsPartOfManageAcademyOpeningsUserRole(
                    GetAuthorizedRole(user, AuthorizedRoles.CanManageAcademyOpenings))) return true;

            if (!string.IsNullOrWhiteSpace(establishmentTypeId) && IsPartOfManageSecureAcademy16To19UserRole(
                    GetAuthorizedRole(user)) && establishmentTypeId.Equals( "46",StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        /// <summary>
        /// A method that returns a PermissionDeniedException.
        /// </summary>
        /// <returns></returns>
        public static PermissionDeniedException GetPermissionDeniedException() =>
            new PermissionDeniedException("Attempt to access a resource without the right permission or value");

        //secure 16-19 academy establishment type Id is 46
        private static bool IsSecureAcademy16To19EstablishmentTypeId(string establishmentTypeId) =>
            establishmentTypeId.Trim().Equals("46", StringComparison.OrdinalIgnoreCase);

        private static bool IsPartOfManageSecureAcademy16To19UserRole(string roleName) =>
            !string.IsNullOrWhiteSpace(roleName) &&
            AuthorizedRoles.CanManageSecureAcademy16To19Openings.Contains(roleName);

        private static bool IsPartOfManageAcademyOpeningsUserRole(string roleName) =>
            !string.IsNullOrWhiteSpace(roleName) && AuthorizedRoles.CanManageAcademyOpenings.Contains(roleName);
    }
}
