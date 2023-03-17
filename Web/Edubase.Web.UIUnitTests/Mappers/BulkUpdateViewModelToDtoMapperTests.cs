using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Mappers;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Mappers
{
    public class BulkUpdateViewModelToDtoMapperTests
    {
        private readonly Mock<HttpPostedFileBase> _fileMock = new Mock<HttpPostedFileBase>();

        public BulkUpdateViewModelToDtoMapperTests()
        {
            _fileMock.Setup(x => x.FileName).Returns("filename.xls");
        }

        [Fact]
        public void EffectiveDateInput_MapsRequiredFields()
        {
            var expectedEffectiveDate = DateTime.Today.Date.AddDays(2);

            var bulkUpdateViewModel = new BulkUpdateViewModel()
            {
                BulkFile = _fileMock.Object,
                BulkUpdateType = BulkUpdateDto.eBulkUpdateType.EduBaseBulkUpload,
                EffectiveDate = new UI.Models.DateTimeViewModel(expectedEffectiveDate),
                CanOverrideCRProcess = true,
                OverrideCRProcess = true
            };

            var resultDto = bulkUpdateViewModel.MapToDto();

            Assert.Equal(expectedEffectiveDate, resultDto.EffectiveDate);
            var expectedOverrideCRProcess = bulkUpdateViewModel.CanOverrideCRProcess && bulkUpdateViewModel.OverrideCRProcess;
            Assert.Equal(expectedOverrideCRProcess, resultDto.OverrideCRProcess);
            Assert.Equal(BulkUpdateDto.eBulkUpdateType.EduBaseBulkUpload, resultDto.BulkFileType);
        }

        [Fact]
        public void EffectiveDateNotInput_SetsEffectiveDateToToday()
        {
            var expectedEffectiveDate = DateTime.Today;

            var bulkUpdateViewModel = new BulkUpdateViewModel()
            {
                BulkFile = _fileMock.Object,
                BulkUpdateType = BulkUpdateDto.eBulkUpdateType.EduBaseBulkUpload,
                CanOverrideCRProcess = true,
                OverrideCRProcess = true
            };

            var resultDto = bulkUpdateViewModel.MapToDto();

            Assert.Equal(expectedEffectiveDate, resultDto.EffectiveDate);
            var expectedOverrideCRProcess = bulkUpdateViewModel.CanOverrideCRProcess && bulkUpdateViewModel.OverrideCRProcess;
            Assert.Equal(expectedOverrideCRProcess, resultDto.OverrideCRProcess);
            Assert.Equal(BulkUpdateDto.eBulkUpdateType.EduBaseBulkUpload, resultDto.BulkFileType);
        }
    }
}
