//using System;
//using System.Net.Http.Headers;
//using Edubase.Services.Texuna.Core;
//using Edubase.Web.UI;
//using Xunit;

//namespace Edubase.Web.UIUnitTests;

//public class IocConfigTests
//{
//    [Theory]
//    [InlineData("", null, null)]
//    [InlineData(" ", null, null)]
//    [InlineData("     ", null, null)]
//    public void CreateLookupClient_ThrowsArgumentNullExceptionForEmptyAddresses(string lookupApiAddress,
//        string lookupApiUsername, string lookupApiPassword)
//    {
//        Assert.Throws<ArgumentNullException>(() => IocConfig.CreateLookupClient(
//            lookupApiAddress,
//            lookupApiUsername,
//            lookupApiPassword));
//    }
//    [Theory]
//    [InlineData("test", null, null)]
//    public void CreateLookupClient_ThrowsUriFormatExceptionForInvalidAddresses(string lookupApiAddress,
//        string lookupApiUsername, string lookupApiPassword)
//    {
//        Assert.Throws<UriFormatException>(() => IocConfig.CreateLookupClient(
//            lookupApiAddress,
//            lookupApiUsername,
//            lookupApiPassword));
//    }

//    [Fact]
//    public void CreateLookupClient_ThrowsNullExceptionFOrNullAddress()
//    {
//        Assert.Throws<ArgumentNullException>(() => IocConfig.CreateLookupClient(
//            null,
//            null,
//            null));
//    }

//    [Theory]
//    [InlineData(@"https://example.com/")]
//    [InlineData(@"https://example.org/api/")]
//    public void CreateLookupClient_ClientHasProvidedAddress(string address)
//    {
//        var sut = IocConfig.CreateLookupClient(address, null, null);

//        Assert.Equal(address, sut.BaseAddress.ToString());
//    }

//    [Theory]
//    [InlineData(null, null)]
//    [InlineData(null, "a")]
//    [InlineData("a", null)]
//    [InlineData("", "")]
//    [InlineData("", "a")]
//    [InlineData("a", "")]
//    public void CreateLookupClient_HasNoAuthCredentialsIfEitherUsernameOrPasswordIsNullOrEmptyString(
//        string username,
//        string password)
//    {
//        var sut = IocConfig.CreateLookupClient(@"https://google.com/", username, password);

//        Assert.Null(sut.DefaultRequestHeaders.Authorization);
//    }

//    [Theory]
//    [InlineData(" ", " ")]
//    [InlineData("test", "pwd")]
//    [InlineData("anotherTest", "anotherPwd")]
//    public void CreateLookupClient_HasAuthCredentialsIfBothUsernameAndPasswordProvided(string username,
//        string password)
//    {
//        var expectedValue = new AuthenticationHeaderValue("Basic",
//            new BasicAuthCredentials(username, password).ToString());

//        var sut = IocConfig.CreateLookupClient(@"https://google.com/", username, password);

//        Assert.Equal(expectedValue, sut.DefaultRequestHeaders.Authorization);
//    }
//}
