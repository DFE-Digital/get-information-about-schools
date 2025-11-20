using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.Admin.Mappings;

namespace Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService;

public record MappingRequest
{
    public MappingRequest(ClientMappingId id, MappingModel model)
    {
        Guard.ThrowIfNull(id, nameof(id));
        ClientId = id;

        Guard.ThrowIfNull(model, nameof(model));
        Mapping = model;
    }
    public ClientMappingId ClientId { get; }
    public MappingModel Mapping { get; }
}

