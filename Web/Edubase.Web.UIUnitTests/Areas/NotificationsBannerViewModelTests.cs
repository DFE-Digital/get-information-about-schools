using Edubase.Data.Entity;
using Edubase.Web.UI.Models.Notifications;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas
{
    public class NotificationsBannerViewModelTests
    {


        [Fact]
        public void DisplayLinkWithText_WhenUrlAndTextProvided()
        {
            var model = new NotificationsBannerViewModel();

            var result = model.sanitizedContentWithLinks(
                "Message",
                "https://test.com",
                "Click me!",
                "",
                "");

            Assert.Equal("Message <br /><a href=\"https://test.com/\" target=\"_blank\" rel=\"noopener noreferrer\">Click me!</a>", result);
        }

        [Fact]
        public void DisplayUrl_WhenTextEmpty()
        {
            var model = new NotificationsBannerViewModel();

            var result = model.sanitizedContentWithLinks(
                "Message",
                "https://test.com",
                "",
                "",
                "");

            Assert.Equal("Message <br />https://test.com/", result);
        }

        [Fact]
        public void DisplayBothLinksCorrectly()
        {
            var model = new NotificationsBannerViewModel();

            var result = model.sanitizedContentWithLinks(
                "Message",
                "https://test.com",
                "Click me!",
                "https://test2.com",
                "Click me again!");

            var expected =
                "Message <br /><a href=\"https://test.com/\" target=\"_blank\" rel=\"noopener noreferrer\">Click me!</a> <br /><a href=\"https://test2.com/\" target=\"_blank\" rel=\"noopener noreferrer\">Click me again!</a>";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Display_MessageContentOnly()
        {
            var model = new NotificationsBannerViewModel();

            var result = model.sanitizedContentWithLinks(
                "Example message",
                "",
                "",
                "",
                "");

            Assert.Equal("Example message", result);
        }

        [Fact]
        public void Set_PopulateMessageAndLink1_remove_br()
        {
            var banner = new NotificationBanner
            {
                Content = "Test message <br /><a href=\"https://test.com\">Click me!</a>"
            };

            var viewmodel = new NotificationsBannerViewModel().Set(banner);

            Assert.Equal("Test message", viewmodel.Content);
            Assert.Equal("https://test.com", viewmodel.LinkUrl1);
            Assert.Equal("Click me!", viewmodel.LinkText1);
            Assert.Null(viewmodel.LinkText2);
            Assert.Null(viewmodel.LinkUrl2);
        }

        [Fact]
        public void Set_PopulateMessageAndTwoLinks()
        {
            var banner = new NotificationBanner
            {
                Content = "Test message <br /><a href=\"https://test.com\">Click me!</a><br /><a href=\"https://test2.com\">Click me again!</a>"
            };

            var viewmodel = new NotificationsBannerViewModel().Set(banner);

            Assert.Equal("Test message", viewmodel.Content);
            Assert.Equal("https://test.com", viewmodel.LinkUrl1);
            Assert.Equal("Click me!", viewmodel.LinkText1);
            Assert.Equal("https://test2.com", viewmodel.LinkUrl2);
            Assert.Equal("Click me again!", viewmodel.LinkText2);
        }
    }
}
