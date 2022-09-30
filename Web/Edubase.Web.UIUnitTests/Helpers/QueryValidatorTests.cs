using Xunit;

namespace Edubase.Web.UI.Helpers.Tests
{
    public class QueryValidatorTests
    {
        [Theory]
        [InlineData(@"this is a valid string")]
        [InlineData(@"special characters ' . - ! ,")]
        [InlineData(@"1")]
        [InlineData(@"2")]
        [InlineData(@"3")]
        [InlineData(@"4")]
        [InlineData(@"5")]
        [InlineData(@"6")]
        [InlineData(@"7")]
        [InlineData(@"8")]
        [InlineData(@"9")]
        [InlineData(@"0")]
        [InlineData(@"q")]
        [InlineData(@"w")]
        [InlineData(@"e")]
        [InlineData(@"r")]
        [InlineData(@"t")]
        [InlineData(@"y")]
        [InlineData(@"u")]
        [InlineData(@"i")]
        [InlineData(@"o")]
        [InlineData(@"p")]
        [InlineData(@"a")]
        [InlineData(@"s")]
        [InlineData(@"d")]
        [InlineData(@"f")]
        [InlineData(@"g")]
        [InlineData(@"h")]
        [InlineData(@"j")]
        [InlineData(@"k")]
        [InlineData(@"l")]
        [InlineData(@"z")]
        [InlineData(@"x")]
        [InlineData(@"c")]
        [InlineData(@"v")]
        [InlineData(@"b")]
        [InlineData(@"n")]
        [InlineData(@"m")]
        [InlineData(@"Q")]
        [InlineData(@"W")]
        [InlineData(@"E")]
        [InlineData(@"R")]
        [InlineData(@"T")]
        [InlineData(@"Y")]
        [InlineData(@"U")]
        [InlineData(@"I")]
        [InlineData(@"O")]
        [InlineData(@"P")]
        [InlineData(@"A")]
        [InlineData(@"S")]
        [InlineData(@"D")]
        [InlineData(@"F")]
        [InlineData(@"G")]
        [InlineData(@"H")]
        [InlineData(@"J")]
        [InlineData(@"K")]
        [InlineData(@"L")]
        [InlineData(@"Z")]
        [InlineData(@"X")]
        [InlineData(@"C")]
        [InlineData(@"V")]
        [InlineData(@"B")]
        [InlineData(@"N")]
        [InlineData(@"M")]
        [InlineData(@"'")]
        [InlineData(@".")]
        [InlineData(@"-")]
        [InlineData(@"!")]
        [InlineData(@",")]
        public void ValidatePlaceSuggestionQuery_ReturnsTrueForValidStrings(string validString)
        {
            Assert.True(QueryValidator.ValidatePlaceSuggestionQuery(validString));
        }

        [Fact]
        public void ValidatePlaceSuggestionQuery_ReturnsTrueFor50CharacterString()
        {
            var text = string.Empty;
            for (var i = 0; i < 50; i++)
            {
                text = text += "a";
            }
            Assert.True(QueryValidator.ValidatePlaceSuggestionQuery(text));
        }

        [Fact]
        public void ValidatePlaceSuggestionQuery_ReturnsFalseFor51CharacterString()
        {
            var text = string.Empty;
            for (var i = 0; i < 51; i++)
            {
                text = text += "a";
            }
            Assert.False(QueryValidator.ValidatePlaceSuggestionQuery(text));
        }

        [Fact]
        public void ValidatePlaceSuggestionQuery_ReturnsFalseIfNull()
        {
            Assert.False(QueryValidator.ValidatePlaceSuggestionQuery(null));
        }

        [Fact]
        public void ValidatePlaceSuggestionQuery_ReturnsFalseIfEmptyString()
        {
            Assert.False(QueryValidator.ValidatePlaceSuggestionQuery(string.Empty));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("      ")]
        [InlineData("       ")]
        [InlineData("              ")]
        public void ValidatePlaceSuggestionQuery_ReturnsFalseIfWhiteSpace(string whitespace)
        {
            Assert.False(QueryValidator.ValidatePlaceSuggestionQuery(whitespace));
        }


        [Theory]
        [InlineData(@"`")]
        [InlineData(@"¬")]
        [InlineData(@"|")]
        [InlineData("\"")]
        [InlineData(@"£")]
        [InlineData(@"$")]
        [InlineData(@"%")]
        [InlineData(@"^")]
        [InlineData(@"&")]
        [InlineData(@"*")]
        [InlineData(@"(")]
        [InlineData(@")")]
        [InlineData(@"_")]
        [InlineData(@"+")]
        [InlineData(@"=")]
        [InlineData(@"[")]
        [InlineData(@"{")]
        [InlineData(@"]")]
        [InlineData(@"}")]
        [InlineData(@";")]
        [InlineData(@":")]
        [InlineData(@"\")]
        [InlineData(@"@")]
        [InlineData(@"#")]
        [InlineData(@"~")]
        [InlineData(@"<")]
        [InlineData(@">")]
        [InlineData(@"/")]
        [InlineData(@"?")]
        public void ValidatePlaceSuggestionQuery_ReturnsFalseForValidStrings(string invalidString)
        {
            Assert.False(QueryValidator.ValidatePlaceSuggestionQuery(invalidString), "This test needs an implementation");
        }
    }
}
