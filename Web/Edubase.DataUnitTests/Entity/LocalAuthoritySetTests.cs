using Edubase.Data.Entity;
using Xunit;

namespace Edubase.DataUnitTests.Entity;

public class LocalAuthoritySetTests
{
    private static readonly int[] expected = [1, 123, 999];

    [Fact]
    public void Ids_RoundTripsThroughBsonData()
    {
        var source = new LocalAuthoritySet { Ids = [1, 123, 999] };
        var restored = new LocalAuthoritySet { IdData = source.IdData };

        Assert.NotEmpty(source.IdData);
        Assert.Equal(expected, restored.Ids);
    }
}
