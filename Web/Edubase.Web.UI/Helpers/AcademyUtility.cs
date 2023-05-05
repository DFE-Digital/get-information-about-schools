using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Search;

namespace Edubase.Web.UI.Helpers
{
    /// <summary>
    /// This is a utility class that helps with some operations regarding Academy establishment type.
    /// </summary>
    public static class AcademyUtility
    {
        private const string EncryptionKey = "12wsdftgh4567mncvb";

        /// <summary>
        /// A method to use to get the words for the title page.
        /// Please note, this method expects the correct Establishment Type Id i.e 46
        /// & correct user i.e. one in the CanManageSecure16To19AcademyOpenings role.
        /// </summary>
        /// <param name="establishmentTypeId"></param>
        /// <param name="isSecure16To19User"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetAcademyOpeningPageTitle(string establishmentTypeId, bool isSecure16To19User)
        {
            if (string.IsNullOrWhiteSpace(establishmentTypeId)) return "Manage academy openings";

            if (!string.IsNullOrWhiteSpace(establishmentTypeId) &&
                IsSecureAcademy16To19EstablishmentTypeId(establishmentTypeId) && isSecure16To19User)
                return "Manage secure academy 16-19 openings";

            throw new ArgumentException("unexpected parameters or values passed");
        }

        /// <summary>
        /// A method to use to filter the passed in Establishments by Establishment Type Id passed in.
        /// NB: The filtering process will only take place if the user is in the CanManageSecure16To19AcademyOpenings role,
        /// else the same collection of Establishment types passed is returned without being filtered.
        /// </summary>
        /// <param name="establishmentTypes"></param>
        /// <param name="establishmentTypeId"></param>
        /// <param name="isSecure16To19User"></param>
        /// <returns></returns>
        public static IEnumerable<EstablishmentLookupDto> FilterEstablishmentsByEstablishmentTypeId(
            IEnumerable<EstablishmentLookupDto> establishmentTypes, string establishmentTypeId,
            bool isSecure16To19User)
        {
            if (isSecure16To19User && !string.IsNullOrWhiteSpace(establishmentTypeId) &&
                int.TryParse(establishmentTypeId, out var estabTypeId) &&
                IsSecureAcademy16To19EstablishmentTypeId(establishmentTypeId))
                establishmentTypes = establishmentTypes.Where(e => e.Id == estabTypeId);

            return establishmentTypes;
        }

        /// <summary>
        /// A method to use to get the correct Establishment filters to use in a search.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static EstablishmentSearchFilters GetEstablishmentSearchFilters(GetEstabSearchFiltersParam param)
        {
            //secure 16-19 academy establishment type Id is 46
            var estabTypeId = -1;
            if ((param.IsSecure16To19User && !string.IsNullOrWhiteSpace(param.EstablishmentTypeId) &&
                 int.TryParse(param.EstablishmentTypeId, out estabTypeId) && estabTypeId != 46)
                || (!param.IsSecure16To19User && !string.IsNullOrWhiteSpace(param.EstablishmentTypeId)))
                throw new ArgumentException("Invalid parameter(s) supplied");

            if (param.IsSecure16To19User && !string.IsNullOrWhiteSpace(param.EstablishmentTypeId) && estabTypeId == 46)
                return new EstablishmentSearchFilters
                {
                    OpenDateMin = param.From,
                    OpenDateMax = param.To,
                    StatusIds = new[] { (int) eLookupEstablishmentStatus.ProposedToOpen },
                    TypeIds = new[] { estabTypeId }
                };

            return new EstablishmentSearchFilters
            {
                OpenDateMin = param.From,
                OpenDateMax = param.To,
                StatusIds = new[] { (int) eLookupEstablishmentStatus.ProposedToOpen },
                EstablishmentTypeGroupIds = new[] { (int) eLookupEstablishmentTypeGroup.Academies }
            };
        }

        /// <summary>
        /// A method to use to encrypt a non null string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncryptValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Contains("_")) value = value.Replace("_", "");

            var encodedBytes = Encoding.Unicode.GetBytes(value);
            string encryptedValue;
            using (var encryptor = Aes.Create())
            {
                SetupEncryptor(encryptor);
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream =
                           new CryptoStream(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        ProcessCryptoStream(cryptoStream, encodedBytes);
                    }

                    encryptedValue = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            //Be wary of encrypted values containing '/' as that value cause problem for Api calls

            return encryptedValue;
        }

        private static void SetupEncryptor(Aes encryptor)
        {
            var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
        }

        private static void ProcessCryptoStream(CryptoStream cryptoStream, byte[] bytes)
        {
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.Clear();
        }

        public static string DecryptValue(string encryptedValue)
        {
            try
            {
                var baseBytes = Convert.FromBase64String(encryptedValue);
                string decryptedValue;
                using (var encryptor = Aes.Create())
                {
                    SetupEncryptor(encryptor);
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream =
                               new CryptoStream(memoryStream, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            ProcessCryptoStream(cryptoStream, baseBytes);
                        }

                        decryptedValue = Encoding.Unicode.GetString(memoryStream.ToArray());
                    }
                }

                return decryptedValue;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"The supplied parameter is compromised: {ex.Message}");
            }
        }

        /// <summary>
        /// A method to use for getting the name of the role of a user if the given user is part of the
        /// 'CanManageSecure16To19AcademyOpenings' role group.
        /// NB: It return null if it's not part of the CanManageSecure16To19AcademyOpenings role group.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetSecureAcademy16To19Role(IPrincipal user)
        {
            var identity = (ClaimsIdentity) user.Identity;
            var roles = identity.Claims.ToList()
                .Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            return roles.FirstOrDefault(role =>
                AuthorizedRoles.CanManageSecure16To19AcademyOpenings.RemoveUnderscore()
                    .Contains(role.RemoveUnderscore()));
        }

        /// <summary>
        /// A method to use to check if a given role name/value is part of the roles in the
        /// 'CanManageSecure16To19AcademyOpenings' role group.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static bool IsPartOfManageSecureAcademy16To19UserRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return false;

            return AuthorizedRoles.CanManageSecure16To19AcademyOpenings.Replace("_", "")
                .Contains(roleName.RemoveUnderscore());
        }

        /// <summary>
        /// A method to use to check if the user has authorization to access a resource limited to those in
        /// the 'CanManageSecure16To19AcademyOpenings' user role.
        /// NB: This method is attempting to prevent manipulation or miss use of query parameters.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool DoesHaveAccessAuthorization(IPrincipal user, string roleName, string establishmentTypeId)
        {
            if (string.IsNullOrWhiteSpace(roleName) && string.IsNullOrWhiteSpace(establishmentTypeId)) return true;

            return IsPartOfManageSecureAcademy16To19UserRole(GetSecureAcademy16To19Role(user));
        }

        /// <summary>
        /// A method that returns an AccessViolationException.
        /// </summary>
        /// <returns></returns>
        public static AccessViolationException GetAccessViolationException() =>
            new AccessViolationException("Attempt to access resource without the right authorization");

        /// <summary>
        /// A method to use to check a roleName is part of those in the
        /// 'CanManageSecure16To19AcademyOpenings' user roles.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static bool IsUserSecureAcademy16To19User(string roleName) =>
            !string.IsNullOrWhiteSpace(roleName) && IsPartOfManageSecureAcademy16To19UserRole(DecryptValue(roleName));

        /// <summary>
        /// A method to use to get the value of an encrypted string EstablishmentTypeId.
        /// </summary>
        /// <param name="establishmentTypeId"></param>
        /// <param name="isUserSecureAcademy16To19"></param>
        /// <returns></returns>
        public static string GetDecryptedEstablishmentTypeId(string establishmentTypeId, bool isUserSecureAcademy16To19)
        {
            if (!string.IsNullOrWhiteSpace(establishmentTypeId) && isUserSecureAcademy16To19)
                return DecryptValue(establishmentTypeId);

            return establishmentTypeId;
        }

        //secure 16-19 academy establishment type Id is 46
        private static bool IsSecureAcademy16To19EstablishmentTypeId(string establishmentTypeId) =>
            establishmentTypeId.Trim().Equals("46", StringComparison.OrdinalIgnoreCase);

        private static string RemoveUnderscore(this string value) => value.Replace("_", "");
    }
}
