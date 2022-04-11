using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Models.Notifications;
using Moq;
using Xunit;

namespace Edubase.Web.UI.Controllers.UnitTests
{
    public class NotificationsControllerTests
    {
        [Fact]
        public async Task NotificationsController_ArchiveExpiredBanner_Returns_GroupedBanners()
        {
            //Arrange
            var ntr = new Mock<INotificationTemplateRepository>();
            var nbr = new Mock<INotificationBannerRepository>();

            var banners = new List<NotificationBanner>()
                {
                    new NotificationBanner() { Content = "Banner1" },
                    new NotificationBanner() { Content = "Banner2" }
                };

            nbr.Setup(x => x.GetExpiredAsync(eNotificationBannerPartition.Current))
                .ReturnsAsync(banners);

            //Act
            var subject = new NotificationsController(ntr.Object, nbr.Object);
            var result = await subject.ArchiveExpiredBanners() as ViewResult;

            var notificationBannerResults = (result?.Model as NotificationsBannersArchiveExpiredViewModel)?.GroupedBanners;

            //Assert
            Assert.Equal(banners.Count, notificationBannerResults.Count);
        }
    }
}
