using System.Collections.Generic;
using System.IO;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.Guidance;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ILaNameCodeFileGenerator
    {
        MemoryStream Generate(IEnumerable<LaNameCodes> rows, eFileFormat fileFormat, string nameColumnHeader);
    }
}
