using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService;
public record ClientMappingId
{
    public ClientMappingId(string identifier)
    {
        Guard.ThrowIfNullOrWhiteSpace(identifier, nameof(identifier));
        Value = identifier;
    }

    public string Value { get; }
}
