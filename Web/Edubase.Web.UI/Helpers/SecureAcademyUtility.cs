using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Search;

namespace Edubase.Web.UI.Helpers
{
    public static class SecureAcademyUtility
    {
        public static string GetAcademyOpeningPageTitle(string establishmentTypeId, bool isSecure16To19User)
        {
            return string.IsNullOrWhiteSpace(establishmentTypeId) ||
                   (!IsSecure16to19AcademyEstablishmentTypeId(establishmentTypeId) && !isSecure16To19User)
                ? "Manage academy openings"
                : "Manage 16-19 secure academy openings";
        }

        public static IEnumerable<EstablishmentLookupDto> FilterEstablishmentsByEstablishmentTypeId(
            IEnumerable<EstablishmentLookupDto> establishmentTypes, string establishmentTypeId,
            bool isSecure16To19User)
        {
            if (isSecure16To19User && !string.IsNullOrWhiteSpace(establishmentTypeId) &&
                int.TryParse(establishmentTypeId, out var estabTypeId))
                establishmentTypes = establishmentTypes.Where(e => e.Id == estabTypeId);

            return establishmentTypes;
        }

        public static EstablishmentSearchFilters GetEstablishmentSearchFilters(GetEstabSearchFiltersParam param)
        {
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


        public static string EncryptValue(string value)
        {
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
            catch (Exception e)
            {
                throw new ArgumentException("The supplied parameter is compromised");
            }
        }
    }
}
