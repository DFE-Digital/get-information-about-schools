using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.IO
{
    public class DirectoryHelper
    {
        public static DirectoryInfo CreateTempDirectory()
            => Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
    }
}
