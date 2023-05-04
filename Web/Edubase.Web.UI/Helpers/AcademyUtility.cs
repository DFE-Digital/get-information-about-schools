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
    public static class AcademyUtility
    {
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
                IsSecure16to19AcademyEstablishmentTypeId(establishmentTypeId) && isSecure16To19User)
                return "Manage secure academy 16-19 openings";

            throw new ArgumentException("unexpected parameters or values passed");
        }

        /// <summary>
        /// A method to use to filter the passed in Establishments by Establishment Type Id passed in.
        /// NB: The filtering process will only take place if the user is in the CanManageSecure16To19AcademyOpenings role
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
                IsSecure16to19AcademyEstablishmentTypeId(establishmentTypeId))
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

        //secure 16-19 academy establishment type Id is 46
        private static bool IsSecure16to19AcademyEstablishmentTypeId(string establishmentTypeId) =>
            establishmentTypeId.Trim().Equals("46", StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// A method to use to encrypt a non null string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncryptValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Contains("_")) value = value.Replace("_","");

            var encryptionKey = "12wsdftgh4567mncvb";
            var encodedBytes = Encoding.Unicode.GetBytes(value);
            string encryptedValue;
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream =
                           new CryptoStream(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encodedBytes, 0, encodedBytes.Length);
                        cryptoStream.Clear();
                    }

                    encryptedValue = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            return encryptedValue;
        }

        public static string DecryptValue(string encryptedValue)
        {
            // if (encryptedValue.Contains("-"))
            //     encryptedValue = encryptedValue.Replace("_", "/");

            var encryptionKey = "12wsdftgh4567mncvb";
            try
            {
                var baseBytes = Convert.FromBase64String(encryptedValue);
                string decryptedValue;
                using (var encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(encryptionKey,
                        new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream =
                               new CryptoStream(memoryStream, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(baseBytes, 0, baseBytes.Length);
                            cryptoStream.Clear();
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

        public static string GetSecureAcademy16To19Role(IPrincipal user)
        {
            var identity = (ClaimsIdentity) user.Identity;
            var roles = identity.Claims.ToList()
                .Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            return roles.FirstOrDefault(role =>
                AuthorizedRoles.CanManageSecure16To19AcademyOpenings.RemoveUnderscore().Contains(role.RemoveUnderscore()));
        }

        public static bool IsSecureAcademy16To19User(IPrincipal user)
        {
            var role = GetSecureAcademy16To19Role(user);
            return AuthorizedRoles.CanManageSecure16To19AcademyOpenings.RemoveUnderscore().Contains(role.RemoveUnderscore());
        }

        public static bool IsSecureAcademy16To19User(string group) =>
            AuthorizedRoles.CanManageSecure16To19AcademyOpenings.Replace("_", "").Contains(group);

        public static bool IsSameSecureAcademy16To19User(IPrincipal user, string group)
        {
            var role = GetSecureAcademy16To19Role(user);
            if (string.IsNullOrWhiteSpace(role)) return false;
            return string.Compare(role.RemoveUnderscore(), group, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string RemoveUnderscore(this string value) => value.Replace("_", "");
    }
}
