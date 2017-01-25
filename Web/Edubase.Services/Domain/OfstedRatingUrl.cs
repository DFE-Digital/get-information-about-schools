using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class OfstedRatingUrl
    {
        private int? _urn;

        public OfstedRatingUrl(int? urn)
        {
            _urn = urn;
        }
        public override string ToString() => _urn != null ? $"http://www.ofsted.gov.uk/oxedu_providers/full/(urn)/{_urn}" : string.Empty;
    }
}
