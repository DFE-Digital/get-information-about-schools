using AutoMapper;
using Xunit;

namespace Edubase.Web.UIUnitTests
{

public class JsonConverterTestModels
{
    public class TestClass
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}

public class JsonTypeConverterTests
{
    private readonly JsonTypeConverter<JsonConverterTestModels.TestClass> _converter =
        new JsonTypeConverter<JsonConverterTestModels.TestClass>();

    [Fact]
    public void ConvertTo_ShouldSerializeObject()
    {
        var obj = new JsonConverterTestModels.TestClass { Name = "John", Value = 42 };

        var json = _converter.ConvertTo(null, null, obj, typeof(string)) as string;

        Assert.NotNull(json);
        Assert.Contains("\"Name\":\"John\"", json);
        Assert.Contains("\"Value\":42", json);
    }

    [Fact]
    public void ConvertFrom_ShouldDeserializeValidJson()
    {
        var json = "{\"Name\":\"Alice\",\"Value\":10}";

        var result = (JsonConverterTestModels.TestClass)_converter.ConvertFrom(null, null, json);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
        Assert.Equal(10, result.Value);
    }

    [Fact]
    public void ConvertFrom_InvalidJson_ShouldReturnDefault()
    {
        var invalid = "{ invalid json }";

        var result = (JsonConverterTestModels.TestClass)_converter.ConvertFrom(null, null, invalid);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertFrom_NullValue_ShouldReturnDefault()
    {
        var result = (JsonConverterTestModels.TestClass)_converter.ConvertFrom(null, null, null);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertTo_NullValue_ShouldReturnNull()
    {
        var result = _converter.ConvertTo(null, null, null, typeof(string));

        Assert.Null(result);
    }
}

public class AutoMapperJsonConverterTests
{
    private readonly ToJsonTypeConverter<JsonConverterTestModels.TestClass> _toJson =
        new ToJsonTypeConverter<JsonConverterTestModels.TestClass>();

    private readonly FromJsonTypeConverter<JsonConverterTestModels.TestClass> _fromJson =
        new FromJsonTypeConverter<JsonConverterTestModels.TestClass>();

    private readonly ResolutionContext _context = default;

    [Fact]
    public void ToJson_ValidObject_ShouldSerialize()
    {
        var obj = new JsonConverterTestModels.TestClass { Name = "Bob", Value = 5 };

        var json = _toJson.Convert(obj, null, _context);

        Assert.NotNull(json);
        Assert.Contains("\"Name\":\"Bob\"", json);
        Assert.Contains("\"Value\":5", json);
    }

    [Fact]
    public void FromJson_ValidJson_ShouldDeserialize()
    {
        var json = "{\"Name\":\"Claire\",\"Value\":8}";

        var result = _fromJson.Convert(json, null, _context);

        Assert.NotNull(result);
        Assert.Equal("Claire", result.Name);
        Assert.Equal(8, result.Value);
    }

    [Fact]
    public void FromJson_InvalidJson_ShouldReturnDefault()
    {
        var invalid = "{ nope json }";

        var result = _fromJson.Convert(invalid, null, _context);

        Assert.Null(result);
    }

    [Fact]
    public void ToJson_NullInput_ShouldReturnNull()
    {
        var result = _toJson.Convert(null, null, _context);

        Assert.Null(result);
    }

    [Fact]
    public void FromJson_NullOrWhitespace_ShouldReturnDefault()
    {
        Assert.Null(_fromJson.Convert(null, null, _context));
        Assert.Null(_fromJson.Convert("", null, _context));
        Assert.Null(_fromJson.Convert("   ", null, _context));
    }
}
}
