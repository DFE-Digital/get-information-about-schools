using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.IO
{
    public class LogTextWriter : StringWriter
    {
        public LogTextWriter()
        {
        }

        public LogTextWriter(StringBuilder sb) : base(sb)
        {
        }

        public LogTextWriter(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        public LogTextWriter(StringBuilder sb, IFormatProvider formatProvider) : base(sb, formatProvider)
        {
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(DateTime.UtcNow + "\t" + value);
        }
    }
}
