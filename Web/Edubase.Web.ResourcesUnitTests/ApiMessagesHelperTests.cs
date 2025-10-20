using System.Reflection;
using System.Resources;
using Xunit;

namespace Edubase.Web.Resources.UnitTests;

public class ApiMessagesHelperTests
{
    [Theory()]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void GetTest_NullEmptyOrWhitespaceReturnsDefaultValue(string key)
    {
        var defaulatValue = "default value";

        var result = ApiMessagesHelper.Get(key, defaulatValue);

        Assert.Equal(defaulatValue, result);
    }

    [Fact()]
    public void GetTest_DefaultDefaultValueIsEmptyString()
    {
        var result = ApiMessagesHelper.Get(null);
        Assert.Equal(string.Empty, result);
    }

    [Fact()]
    public void GetTest()
    {
        var ApiMessageHelperAssembly = Assembly.GetAssembly(typeof(ApiMessagesHelper));
#pragma warning disable CS8604 // Possible null reference argument. Test will fail if null
        var resourceManager = new ResourceManager("Edubase.Web.Resources.ApiMessages", ApiMessageHelperAssembly);
#pragma warning restore CS8604 // Possible null reference argument. Test will fail if null

        var recourceSet = resourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

#pragma warning disable CS8602 // Dereference of a possibly null reference. Test will fail if null
        var dictionaryEnumerator = recourceSet.GetEnumerator();
#pragma warning restore CS8602 // Dereference of a possibly null reference. Test will fail if null

        var enumerationCount = 0;

        //check each entry in the resource file, that the method returns the matching value for the key.
        while (dictionaryEnumerator.MoveNext())
        {
            enumerationCount++;
            var result = ApiMessagesHelper.Get(dictionaryEnumerator.Key as string);

            Assert.Equal(dictionaryEnumerator.Value as string, result);
        }

        //ensure that the enumarator has had at least one value
        Assert.NotEqual(0, enumerationCount);
    }
}
