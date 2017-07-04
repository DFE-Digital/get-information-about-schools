using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class FileDownloadDto
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? FileSizeInBytes { get; set; }
    }
}
